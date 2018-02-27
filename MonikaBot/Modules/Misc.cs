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
    public class Misc : BaseModule
    {
        public Misc(DiscordSocketClient client, CommandService service)
            : base(client, service) { }

        public string prefix = "!";

        [Command("Help")]
        public async Task HelpAsync()
        {
            await ReplyAsync(_client.CurrentUser.Username);
            var dmChanenl = await Context.User.GetOrCreateDMChannelAsync();

            var builder = new EmbedBuilder()
            {
                Color = Color.Red,
                Description = "List of commands"
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    }
                }

                if (!string.IsNullOrEmpty(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });

                }
            }
            await dmChanenl.SendMessageAsync("", false, builder.Build());
        }

    }

}
