using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SirusBot.Modules
{
    public class MemesModule : ModuleBase<SocketCommandContext>
    {

        private ILogger<MemesModule> _logger;
        
        public MemesModule(ILogger<MemesModule> logger)
            => _logger = logger;

        [Command("meme")]
        [Alias("reddit")]
        public async Task memeAsync(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");
            if (!result.StartsWith("["))
            {
                await ReplyAsync($"Subreddit {subreddit} does not exist");
            }
            else
            {
                JArray arr = JArray.Parse(result);
                JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

                var builder = new EmbedBuilder()
                    .WithImageUrl(post["url"].ToString())
                    .WithColor(new Color(148, 5, 0))
                    .WithTitle(post["title"].ToString())
                    .WithUrl("https://reddit.com" + post["permalink"].ToString())
                    .WithFooter($"🗨{post["num_comments"]} ⬆️{post["ups"]}");

                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }

            _logger.LogInformation($"{Context.User.Username} executed meme command.");
        }


        [Command("is")]
        public async Task IsAsync([Remainder] string question)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("https://yesno.wtf/api");
            JObject yesno = JObject.Parse(result);

            var builder = new EmbedBuilder()
                .WithImageUrl(yesno["image"].ToString())
                .WithColor(new Color(148, 5, 0))
                .WithTitle(yesno["answer"].ToString());

            var embed = builder.Build();
            await ReplyAsync(null, false, embed);

            _logger.LogInformation($"{Context.User.Username} executed is command.");
        }
    }
}
