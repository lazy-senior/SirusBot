using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirusBot.Modules
{
    public class GeneralModule : ModuleBase  
    {
        private ILogger<GeneralModule> _logger;

        public GeneralModule(ILogger<GeneralModule> logger)
            => _logger = logger;

        [Command("dieNO")]
        public async Task PingAsync()
        {
            await ReplyAsync("How boring and small.");
            _logger.LogInformation($"{Context.User.Username} executed die command.");
        }

        [Command("info")]
        public async Task InfoAsync(SocketGuildUser user = null)
        {
            EmbedBuilder builder;
            if (user == null)
            {
                builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription("A fellow exile? One more fool under Zana's sway.")
                    .WithColor(new Color(148, 5, 0))
                    .AddField("User Id", Context.User.Id, true)
                    .AddField("Created At", Context.User.CreatedAt.ToString("d"), true)
                    .AddField("Joined At", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("d"), true)
                    .AddField("Roles", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)));
            }
            else if(user.Id == Context.Client.CurrentUser.Id)
            {
                builder = new EmbedBuilder()
                    .WithDescription("You assumed I share the same weakness as my fellows. I do not.")
                    .WithColor(new Color(148, 5, 0));
            }
            else
            {
                builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription("A fellow exile? One more fool under Zana's sway.")
                    .WithColor(new Color(148, 5, 0))
                    .AddField("User Id", user.Id, true)
                    .AddField("Created At", user.CreatedAt.ToString("d"), true)
                    .AddField("Joined At", user.JoinedAt.Value.ToString("d"), true)
                    .AddField("Roles", string.Join(" ", user.Roles.Select(x => x.Mention)));
            }

            var embed = builder.Build();
            await ReplyAsync(null, false, embed);

            _logger.LogInformation($"{Context.User.Username} executed info command.");
        }


        [Command("server")]
        public async Task ServerAsync()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("You want the Atlas? Take it. It's yours. But Oriath? Oriath I will burn to the ground. Perhaps the suffering of my fellow citizens will finally stir something.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(148, 5, 0))
                .AddField("Created At", Context.Guild.CreatedAt.ToString("d"), true)
                .AddField("Member Count", (Context.Guild as SocketGuild).MemberCount + " members", true)
                .AddField("Online Count", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count() + " members", true);
            var embed = builder.Build();
            await ReplyAsync(null, false, embed);

            _logger.LogInformation($"{Context.User.Username} executed server command.");
        }
    }
}
