using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Victoria;

namespace SirusBot.Services
{
    public class CommandHandler : InitializedService
    {

        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly LavaNode _lavaNode;


        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfiguration config, IServiceProvider provider, LavaNode lavaNode)
        {
            _provider = provider;
            _client = discord;
            _service = commands;
            _config = config;
            _lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.Ready += OnReadyAsync;
            _client.JoinedGuild += OnJoinedGuildAsync;
            _client.MessageReceived += OnMessageReceivedAsync;
            _service.CommandExecuted += OnCommandExecutedAsync;

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
        }

        private async Task OnMessageReceivedAsync(SocketMessage arg)
        {

            Console.WriteLine("MessageReceived");

            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            Console.WriteLine("firstCheck");

            int pos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref pos) && !message.HasMentionPrefix(_client.CurrentUser, ref pos)) return;

            Console.WriteLine("secondCheck");

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, pos, _provider);
            
        }

        private async Task OnJoinedGuildAsync(SocketGuild guild)
        {
            
            var builder = new EmbedBuilder()
                    .WithTitle($"{guild.Name} I will burn to the ground.")
                    .WithImageUrl("https://i.ytimg.com/vi/U7LqV21FKVk/maxresdefault.jpg")
                    .WithDescription("My friends are dead, and now you come to me. Is it bravery? Or foolishness?")
                    .WithColor(new Color(148, 5, 0))
                    .AddField("Music", "!join, !play, !stop, !skip. !pause, !resume, !queue")
                    .AddField("Memes", "!meme, !reddit");

            var embed = builder.Build();
            await guild.DefaultChannel.SendMessageAsync(null, false, embed);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if(command.IsSpecified && !result.IsSuccess)
            {
                await context.Channel.SendMessageAsync($"Error {result}");
            }
        }

    }
}
