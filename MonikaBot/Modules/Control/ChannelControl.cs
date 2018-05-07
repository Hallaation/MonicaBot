using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Modules.Control
{
    public class ChannelControl : BaseModule
    {
        public ChannelControl(DiscordSocketClient client, CommandService service)
          : base(client, service) { }


        [Command("MoveChannel", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        public async Task MoveChannelUsersAsync(ulong aTargetChannelID)
        {
            IGuildUser contextUser = Context.Guild.GetUser(Context.User.Id);
            SocketVoiceChannel channel = Context.Guild.GetVoiceChannel(contextUser.VoiceChannel.Id);

            foreach (IGuildUser channelUser in channel.Users)
            {
                await channelUser.ModifyAsync(x => x.Channel = Context.Guild.GetVoiceChannel(aTargetChannelID));
                Console.WriteLine(channelUser);
            }
            await Task.CompletedTask;
        }

    }
}
