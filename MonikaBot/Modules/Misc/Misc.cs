using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;
        public string prefix = "!";

        public MiscModule(CommandService service)
        {
            _service = service;
            prefix = Services.Settings.LoadFile().Prefix;
        }

 
        [Command("Help")]
        public async Task HelpAsync()
        {
            
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
