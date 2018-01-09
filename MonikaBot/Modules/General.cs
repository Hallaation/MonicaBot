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
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Hello World");
        }

        [Command("Embed")]
        public async Task EmbedTest()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Ping")
                .WithDescription("This is an embedded ping")
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder);
        }
    }
}
