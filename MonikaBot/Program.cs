using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using MonikaBot.Services;
using Newtonsoft.Json;

namespace MonikaBot
{
    class MonikaBot
    {
        static void Main(string[] args) => new MonikaBot().RunBotAsync().GetAwaiter().GetResult();

        private Random _random;
        private Settings _settings;
        private DiscordSocketClient _client; //connection to discord
        private CommandService _commands;
        
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            await LoadSettings();
            //Bot setup
            _random = new Random(Guid.NewGuid().GetHashCode());
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //the bot token
            //should change this to a file stored at the program.
            string botToken = _settings.Token;

            //event subscriptions
            _client.Log += Log;

            //Register command handling
            await RegisterCommandsAsync();

            try
            {
                await _client.LoginAsync(TokenType.Bot, botToken);
                await _client.StartAsync();
                await _client.SetGameAsync("Just Monika");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(-1);
        }

        //A Task to subscribe to when the client logs in
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        //Registers Command handler for any messages recieved;
        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        //Handles any commands that come in
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot)
            {
                return;
            }
            int argPos = 0;

            //if the prefix is used, or the bot is mentioned
            if (message.HasStringPrefix(_settings.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(context.Message);
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

        private async Task Run()
        {
            //while (true)
            //{
            //    Console.WriteLine(_client.GetGuild(0).Name);
            //    await Task.Delay(_random.Next(50, 100));
            //
            //}
        }

        //Used to get bot token and prefix from file or user input if file doesn't exist
        private async Task LoadSettings()
        {
            if ((_settings = Settings.LoadFile()) == null)
            {
                string Token, Prefix;

                Console.WriteLine("Give bot token");
                Token = Console.ReadLine();
                Console.WriteLine("Give bot prefix");

                if ((Prefix = Console.ReadLine()).Length < 1)
                {
                    Prefix = "!";
                }
                _settings = new Settings(Token, Prefix);
                _settings.SaveFile();
            }
            await Task.CompletedTask;
        }

    }

}
