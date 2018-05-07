using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Modules
{
    public class BaseModule : ModuleBase<SocketCommandContext>
    {
        protected readonly DiscordSocketClient _client;
        protected readonly CommandService _service;

        public BaseModule(){}

        public BaseModule(DiscordSocketClient discordSocketClient = null, CommandService commandService = null)
        {
            _client = discordSocketClient;
            _service = commandService;
        }

        protected override void BeforeExecute(CommandInfo command)
        {

        }
    }
}
