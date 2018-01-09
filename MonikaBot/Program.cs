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
using MonikaBot.Extensions;
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
            _client = new DiscordSocketClient();
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

            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await _client.SetGameAsync("Just Monika");

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

            if (message is null || message.Author.IsBot)
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
                    Console.WriteLine(argPos);
                    Console.WriteLine(context.Message);
                    Console.WriteLine(result.ErrorReason);
                }
                Console.WriteLine(context.Message);
                Console.WriteLine(argPos);
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

        private async Task LoadSettings()
        {
            if (File.Exists("./settings.json"))
            {
                using (StreamReader file = File.OpenText("./settings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer(); //make serializer

                    //read the file until it ends
                    string json = "";

                    json += file.ReadLine(); //read the jsonfile

                    _settings = JsonConvert.DeserializeObject<Settings>(json);
                    //deserialize and set it
                    //_settings = JsonConvert.DeserializeObject<Settings>(json);

                }
            }
            else
            {
                _settings = new Settings();
                Console.WriteLine("Give bot token");
                _settings.Token = Console.ReadLine();
                Console.WriteLine("Give bot prefix");
                string prefix;
                if ((prefix = Console.ReadLine()).Length > 0)
                {
                    Console.WriteLine(prefix);
                    _settings.Prefix = prefix;
                }

                using (StreamWriter file = new StreamWriter("./settings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Console.WriteLine(_settings.Token);
                    string jsonData = JsonConvert.SerializeObject(_settings);
                    file.WriteLine(jsonData);
                    serializer.Serialize(file, typeof(Settings));
                }
            }
            await Task.CompletedTask;
        }
    }

}
