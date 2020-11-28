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
     
        /// <param name="commits"></param>
        /// <returns></returns>
        Task BroadcastAllGuilds(List<Commit> commits);


        /// <returns></returns>
        Task CheckCommits();
    }


    public class BroadcastService: IBroadcastService
    {
        private readonly ILogger<BroadcastService> _logger;
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly BotContext _context;

        public int LastCommitId { get; set; } = 0;

        /// <summary>
        /// Creeates BroadCastServer
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <param name="httpClient"></param>
        /// <param name="context"></param>
        public BroadcastService(ILogger<BroadcastService> logger, DiscordSocketClient client, HttpClient httpClient, BotContext context)
        {
            _logger = logger;
            _client = client;
            _httpClient = httpClient;
            _context = context;

        }

        /// <summary>
        /// Broadcast commits to all current guilds
        /// </summary>
        /// <param name="commits"></param>
        /// <returns></returns>

        public async Task BroadcastAllGuilds(List<Commit> commits)
        {
            // get all channels in database
            var channels = _context.Channels.ToListAsync().Result;
            bool hasChanges = true;

            // loop all channels
            foreach (var item in channels)
            {
                // check if channel exists
                // if it doesn't then remove it from database

                var channel = _client.GetChannel(item.ChannelId) as IMessageChannel;
                if ( channel == null )
                {
                    // are we still in the guild even?
                    if (_client.GetGuild(item.GuildId) == null)
                    {
                        // nope we're not. Delete everything
                        // This should already be handled when the bot leaves a server
                        // but we will check it here too?

                        _context.Remove(item);

                        hasChanges = true;

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

                    continue;
                }

                // actually message the channel
                await channel.SendMessageAsync($"A new commit!");
            }

            if (hasChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check the commits page of Garry's Mod
        /// </summary>

        public async Task CheckCommits()
        {
            // get the commit log
            var result = await _httpClient.GetFromJsonAsync<Result>("/r/Garrys Mod?format=json");

            // make sure commit log is actually valid? dead website?
            // todo: add http status code
            if (result != null)
            {
                var lastestCommit = result.results.First();

                if ( Convert.ToInt32(lastestCommit.Changeset) != LastCommitId )
                {
                    // we have changes
                    // let's broadcast them and do some updating
                    int toSelect = Convert.ToInt32(lastestCommit.Changeset) - LastCommitId;

                    var commits = result.results.Take(toSelect).ToList();

                    await BroadcastAllGuilds(commits);
                }

            }

        }
    }
}
