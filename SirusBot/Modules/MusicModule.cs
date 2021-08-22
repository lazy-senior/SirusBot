using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace SirusBot.Modules
{
    public class MusicModule : InteractiveBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("Join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "I'm already connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You must be connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);

                var builder = new EmbedBuilder()
                    .WithDescription("Is life really interesting enough to warrant all this pain?")
                    .AddField("Channel", voiceState.VoiceChannel.Name, true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();

                var message = await ReplyAsync(null, false, embed);

            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "Please provide search terms.", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await JoinAsync();
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(searchQuery);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", $"I wasn't able to find anything for `{searchQuery}`.", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            var track = searchResponse.Tracks[0];

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                player.Queue.Enqueue(track);
                await ReplyAsync($"Enqueued: **{track.Title}**");
            }
            else
            {
                await player.PlayAsync(track);
                await ReplyAsync($"Now Playing: **{track.Title}**");
            }
        }

        [Command("Skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You must be connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "I'm not connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You are not in the same voicechannel as me!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("No items in the queue");
                return;
            }

            await player.SkipAsync();
            await ReplyAsync($"Now playing **{player.Track.Title}**!");

        }

        [Command("Pause", RunMode = RunMode.Async)]
        public async Task PauseAsync()
        {
            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel == null)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You must be connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "I'm not connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You are not in the same voicechannel as me!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync($"Player already paused!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync($"Paused the music!");


        }

        [Command("Resume", RunMode = RunMode.Async)]
        public async Task ResumeAsync()
        {
            var voiceState = Context.User as IVoiceState;

            if (voiceState?.VoiceChannel == null)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You must be connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "I'm not connected to a voice channel!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                var builder = new EmbedBuilder()
                    .WithDescription("Do you yet see the futility of your efforts?")
                    .AddField("Error", "You are not in the same voicechannel as me!", true)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync($"Player already playing!");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync($"Resumed the music!");


        }

        [Command("Queue", RunMode = RunMode.Async)]
        public async Task QueueAsync()
        {
            var pager = new PaginatedMessage();
            
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.Queue.Count > 0)
            {
                var message = "```";
                for (int i = 0; i < player.Queue.Count; i++)
                {
                    message += $"{i+1}\t{player.Queue.ElementAt(i).Title}\t{player.Queue.ElementAt(i).Duration}\n";
                }
                message += "```";
                var builder = new EmbedBuilder()
                    .WithTitle("Queue")
                    .WithDescription(message)
                    .WithColor(new Color(148, 5, 0));
                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }
        }
    }
}
