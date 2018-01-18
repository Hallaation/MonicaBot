using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Services
{
    class StartupService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly Settings _settings;

        public StartupService(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
            _settings = Settings.LoadFile();
        }

        public async Task StartAsync()
        {
            string botToken = _settings.Token;

            await _client.LoginAsync(Discord.TokenType.Bot, botToken);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly()); //load modules
        }
    }
}
