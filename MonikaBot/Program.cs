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
            //checks to see if a settings file exists
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
                .AddSingleton<StartupService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();

            //event subscriptions
            _client.Log += Log;

            //Register command handling
            await _services.GetRequiredService<StartupService>().StartAsync(); //setup starting up bot
            _services.GetRequiredService<CommandHandler>(); //setup command handler
            await Task.Delay(-1);
        }

        //A Task to subscribe to when the client logs in
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
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
