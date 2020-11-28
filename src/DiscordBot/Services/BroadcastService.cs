using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Context;
using DiscordBot.Objects;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services
{
    public interface IBroadcastService
    {
        Task BroadcastAllGuilds();

        Task CheckCommits();
    }


    public class BroadcastService: IBroadcastService
    {
        private readonly ILogger<BroadcastService> _logger;
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly BotContext _context;

        public int LastCommitId { get; set; } = 0;

        public BroadcastService(ILogger<BroadcastService> logger, DiscordSocketClient client, HttpClient httpClient, BotContext context)
        {
            _logger = logger;
            _client = client;
            _httpClient = httpClient;
            _context = context;

        }

        public async Task BroadcastAllGuilds()
        {
            // get all channels in database
            var channels = _context.Channels.ToListAsync().Result;

            // loop all channels
            foreach (var item in channels)
            {
                // check if channel exists
                // if it doesn't then remove it from database

                var channel = _client.GetChannel(item.ChannelId) as IChannel;
                if ( channel == null )
                {
                    // are we still in the guild even?
                    if (_client.GetGuild(item.GuildId) == null)
                    {
                        // nope we're not. Delete everything
                        // This should already be handled when the bot leaves a server
                        // but we will check it here too?

                        continue;
                    }

                    // let's just notify that we tried to send a message but couldn't?
                    // TODO: remove this?
                    try
                    {
                        await _client.GetGuild(item.GuildId).DefaultChannel
                            .SendMessageAsync(
                                "I tried to send a commit but I couldn't. Please setup a commits channel!");
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Critical, ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Check the commits page of Garry's Mod
        /// </summary>

        public async Task CheckCommits()
        {
            using var result = _httpClient.GetFromJsonAsync<Result>("/r/Garrys Mod?format=json");
        }
    }
}
