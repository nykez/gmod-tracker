﻿using System;
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
using DiscordBot.Models;
using System.IO;

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
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly BotContext _context;


        public int LastCommitId { get; set; } =  7653;

        /// <summary>
        /// Creeates BroadCastServer
        /// </summary>
        /// <param name="client"></param>
        /// <param name="httpClient"></param>
        /// <param name="context"></param>
        public BroadcastService(DiscordSocketClient client, HttpClient httpClient, BotContext context)
        {
            _client = client;
            _httpClient = httpClient;
            _context = context;

            GetLastestCommitContext();
        }

        
        // temp

        private async void GetLastestCommitContext()
        {
            try
            {
                if (!System.IO.File.Exists(@"lastcommit.txt"))
                    return;

                using (StreamReader reader = new StreamReader(@"lastcommit.txt"))
                {
                    if (reader.Peek() != -1)
                    {
                        LastCommitId = Convert.ToInt32(await reader.ReadLineAsync());
                    }
                    else
                    {
                        await CheckCommits();
                    }

                }
            }
            catch( FileNotFoundException ex )
            {
                // Write error.
                Console.WriteLine(ex);
            }
        }

        // temp
        private void WriteLastestCommitContext()
        {
            using (StreamWriter writer = System.IO.File.CreateText(@"lastcommit.txt"))
            {
                writer.WriteLine(LastCommitId);
            }
        }

        public async Task BroadcastAllGuilds(List<Commit> commits)
        {
            if (commits == null)
                return;

            // get all channels in database
            var channels = _context.Channels.ToListAsync().Result;
            bool hasChanges = true;

            List<Embed> builtEmbeds = new List<Embed>();


            foreach( var item in commits )
            {
                var embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = item.User.Name,
                        IconUrl = item.User.Avatar,
                    },
                    Color = Color.Blue,
                    ThumbnailUrl = _client.CurrentUser.GetAvatarUrl(),

                }.WithFooter(footer => footer.Text = $"{item.Repo} • {item.Branch}").WithTimestamp(item.Created);

                embed.AddField(item.User.Name, $"[{item.Message}]({$"https://commits.facepunch.com/{item.Id}"})").WithUrl("https://www.twitch.tv/asmongold");

                builtEmbeds.Add(embed.Build());
            }

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
                    // Also just remove any channels assoicated with this guild. they've been deleted or we can't see them
                    // TODO: Remove this?
                    try
                    {
                        _context.Remove(item);
                        hasChanges = true;

                        await _client.GetGuild(item.GuildId).DefaultChannel
                            .SendMessageAsync(
                                "I tried to send a commit but I couldn't. Please setup a commits channel!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    continue;
                }

                // wtf
                builtEmbeds.Reverse();

                // actually message the channel with the list of embeds
                foreach(var emb in builtEmbeds)
                    await channel.SendMessageAsync(embed: emb);

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

                    if (commits.Count == 0)
                        return;

                    LastCommitId = Convert.ToInt32(lastestCommit.Changeset);

                    WriteLastestCommitContext();

                    await BroadcastAllGuilds(commits);
                }

            }

        }
    }
}
