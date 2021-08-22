using Discord;
using Discord.Addons.Interactive;
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
    public class ModerationModule : InteractiveBase<SocketCommandContext>
    {

        private ILogger<ModerationModule> _logger;

        public ModerationModule(ILogger<ModerationModule> logger)
            => _logger = logger;

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeAsync(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var builder = new EmbedBuilder()
                .WithTitle("Purge")
                .WithDescription("Everything ends. Everything crumbles. Everything.")
                .AddField("Messages deleted", messages.Count(), true)
                .WithColor(new Color(148, 5, 0));
            var embed = builder.Build();

            var message = await ReplyAsync(null, false, embed);
            
            _logger.LogInformation($"{Context.User.Username} executed purge command.");
        }
    }
}
