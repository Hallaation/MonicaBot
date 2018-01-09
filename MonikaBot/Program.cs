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

namespace MonikaBot
{
    class MonikaBot
    {

        static void Main(string[] args) => new MonikaBot().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client; //connection to discord
        private CommandService _commands;
        private IServiceProvider _services;


        public async Task RunBotAsync() 
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();


            //the bot token
            //should change this to a file stored at the program.
            string botToken = "!!";

            //event subscriptions
            _client.Log += Log;

            //Register command handling
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();

            await Run();
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
            if (message.HasStringPrefix("monika!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

        private async Task Run()
        {
            while (true)
            {
                Console.WriteLine("Running");
                await Task.Delay(1000);
            }
        }
    }

}
