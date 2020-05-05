using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace twat
{
    class Program
    {
        private DiscordSocketClient _client;

        private readonly string CommandPrefix = "twat";

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.dev.json")
                .Build();

            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            // Remember to keep token private or to read it from an 
            // external source! In this case, we are reading the token 
            // from an environment variable. If you do not know how to set-up
            // environment variables, you may find more information on the 
            // Internet or by using other methods such as reading from 
            // a configuration.
            await _client.LoginAsync(TokenType.Bot,
                configuration.GetSection("DiscordToken").Value);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }


        private async Task MessageReceived(SocketMessage message)
        {
            Console.WriteLine($"{message.Channel} => {message.Content.ToString()} from {message.Author}");
            var command = message.Content.Split(' ');
            if (command[0].ToLower() == CommandPrefix)
            {
                if (command[1].ToLower() == "pp")
                {
                    // await message.Channel.SendMessageAsync("Incoming command");

                    if (message.MentionedUsers.Count == 0)
                    {
                        await CreateAndDistrubutePP(message, (SocketGuildUser) message.Author);
                    }

                    foreach (var MentionedUser in message.MentionedUsers)
                    {
                        await CreateAndDistrubutePP(message, (SocketGuildUser) MentionedUser);
                    }

                    return;
                }

                if (command[1] == "simp")
                {
                    string[] frames = new[] {"⣾", "⣽", "⣻", "⢿", "⡿", "⣟", "⣯", "⣷"};

                    RestUserMessage restUserMessage = await message.Channel.SendMessageAsync("Detecting Simp");

                    foreach (var frame in frames)
                    {
                        await restUserMessage.ModifyAsync(msg => msg.Content = frame);
                        Thread.Sleep(1000);
                    }

                    await restUserMessage.ModifyAsync(msg => msg.Content = "Detected Simp");
                }

                if (command[1] == "embed")
                {
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle("PP Size Machine");
                    builder.WithDescription("SkrubLord's pp \n new Line");
                    builder.WithCurrentTimestamp();
                    builder.WithColor(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
                    await message.Channel.SendMessageAsync("", false, builder.Build());
                    return;
                }

                if (command[1] == "help")
                {
                    await message.Channel.SendMessageAsync(" -> twat pp @UserName");
                    return;
                }
            }
        }

        private async Task CreateAndDistrubutePP(SocketMessage message, SocketGuildUser User)
        {
            int ppLength;
            if (User.Roles.FirstOrDefault(x => x.Name == "SmallPP") != null)
            {
                Console.WriteLine("User has small pp");
                ppLength = new Random().Next(0, 3);
            }
            else if (User.Roles.FirstOrDefault(x => x.Name == "BigPP") != null)
            {
                ppLength = new Random().Next(8, 15);
            }
            else
            {
                ppLength = new Random().Next(0, 10);
            }

            string pp = "8";
            for (int i = 0; i < ppLength; i++)
            {
                pp += "=";
            }

            pp += "D";

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("PP Size Machine");
            builder.WithDescription($"{User}'s pp \n {pp}");
            builder.WithCurrentTimestamp();
            builder.WithColor(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
            await message.Channel.SendMessageAsync("", false, builder.Build());
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}