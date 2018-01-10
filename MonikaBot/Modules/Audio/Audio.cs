using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Audio;

using MonikaBot.Modules.Audio.Services;

namespace MonikaBot.Modules.Audio
{
    class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;

		public AudioModule(AudioService service)
        {
            _service = service;
        }

		//join command, run async so it doesn't stop everything else
		[Command("join", RunMode = RunMode.Async)]
		public async Task JoinAsync()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }
		
		[Command("leave", RunMode = RunMode.Async)]
		public async Task LeaveAsync()
        {
            await _service.LeaveAudio(Context.Guild);
        }

		[Command("play", RunMode = RunMode.Async)]
		public async Task PlayAsync([Remainder] string song)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }
    }
}
