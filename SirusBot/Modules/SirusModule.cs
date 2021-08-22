using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace SirusBot.Modules
{
    public class SirusModule : ModuleBase<SocketCommandContext>
    {

        private readonly LavaNode _lavaNode;

        public SirusModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("die", RunMode = RunMode.Async)]
        public async Task DieAsync()
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;

            var dir = Directory.GetCurrentDirectory();
            var directory = new System.IO.DirectoryInfo("C:\\Users\\User\\source\\repos\\SirusBot\\");
            var audioFile = $"{directory.FullName}die.mp3";

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i {audioFile} - ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);

            var output = ffmpeg.StandardOutput.BaseStream;
            var connection = await channel.ConnectAsync();
            var discord = connection.CreatePCMStream(AudioApplication.Voice);
           
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
        }
    }
}
