using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Audio;
namespace MonikaBot.Modules.Audio.Services
{
    class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectdChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        //guild = guild to join, target = voice channel to join
        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client; //The client trying to connect (this bot)

            //If I found a connected channel, associated with the guild
            if (ConnectdChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            //If the guild im targeting isnt the guilds voice channel return?
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            //Connect to the voice channel
            var audioClient = await target.ConnectAsync(); 

            if (ConnectdChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectdChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            //TODO Get a full path to the file if the value of 'path' is only a filename.
            if (File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectdChannels.TryGetValue(guild.Id, out client))
            {
                using (var output = CreateStream(path).StandardOutput.BaseStream)
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await output.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s161e -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

    }
}
