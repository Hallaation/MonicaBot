using Discord;
using Discord.WebSocket;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Services
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly Settings _settings;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _settings = Settings.LoadFile();

            _client = client;
            _commands = commands;
            _services = services;

            _client.MessageReceived += HandleCommandsAsync;
        }

        public async Task HandleCommandsAsync(SocketMessage arg)
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
    }
}
