﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Context;
using DiscordBot.Models;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Name("General Commands")]
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        private readonly BotContext _context;
        private readonly IBroadcastService _broadcastService;

        // Dependency Injection will fill this value in for us

        public PublicModule(BotContext context, IBroadcastService broadcastService)
        {
            _context = context;
            _broadcastService = broadcastService;
        }

        [Command("ping")]
        [Summary("hello world?")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        [Summary("Returns information about a user")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            // create emebd: add author, color, footer, and thumbnail to it. Timestamp it
            var embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = user.Username,
                    IconUrl = user.GetAvatarUrl()
                },
                Color = Color.Blue,
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Requested by {Context.User.Username}",
                    IconUrl = Context.User.GetAvatarUrl()
                    
                },
                ThumbnailUrl = user.GetAvatarUrl()

            }.WithCurrentTimestamp();

            // add all user roles to embed
            var rolesField = new EmbedFieldBuilder();
            rolesField.Name = "Roles";
            foreach (var role in ((SocketGuildUser) user).Roles)
            {
                rolesField.Value += $"{role.Name}\n";
            }

            // create a creadtedat field
            var createdAtField = new EmbedFieldBuilder()
            {
                Name = "Created At",
                Value = user.CreatedAt.DateTime
            };

            // create a status field
            var statusField = new EmbedFieldBuilder()
            {
                Name = "Status",
                Value = user.Status
            };


            // add fields
            embed.AddField(rolesField);
            embed.AddField(createdAtField);
            embed.AddField(statusField);

            

            await ReplyAsync(embed: embed.Build());
        }
        

        [Command("deletechannel")]
        [Summary("Deletes a channel. Stopping commits to that channel.")]
        public async Task DeleteChannel(IChannel channel = null)
        {
            // if channel somehow bypasses the channel exeception
            if (channel == null)
            {
                await ReplyAsync(
                    $"{MentionUtils.MentionUser(Context.User.Id)} Could not find a valid channel. Try using channel mention instead.");
                return;
            }

            var guildChannel = await _context.Channels.FirstOrDefaultAsync(c => c.ChannelId == channel.Id);

            if (guildChannel != null)
            {
                _context.Remove(guildChannel);
                await _context.SaveChangesAsync();

                await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} Channel has been removed from broadcast list.");
            }
        }

        [Command("setchannel")]
        [Summary("Sets the channel to broadcast commits to.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetChannel(IChannel channel = null)
        {
            // if channel somehow bypasses the channel exeception
            if (channel == null)
            {
                await ReplyAsync(
                    $"{MentionUtils.MentionUser(Context.User.Id)} Could not find a valid channel. Try using channel mention instead.");
                return;
            }

            var guildChannel =  await _context.Channels.FirstOrDefaultAsync(c => c.ChannelId == channel.Id || c.GuildId == Context.Guild.Id);

            if (guildChannel != null)
            {
                await ReplyAsync(
                    $"{MentionUtils.MentionUser(Context.User.Id)} Guild already has a channel set for commits. \n(Use **DeleteChannel** command to remove it) {MentionUtils.MentionChannel(guildChannel.ChannelId)}");
                return;
            }
                

            await _context.Channels.AddAsync(new RegisteredChannel {ChannelId = channel.Id, GuildId = Context.Guild.Id});
            await _context.SaveChangesAsync();
            
            // we have a found channel: process it and save it into database to broadcast too
            // if we broadcast to the channel, and it's not valid then remove it from the database
            // and alert an admin?

            await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} set Garry's Mod commit channel to {MentionUtils.MentionChannel(channel.Id)}!");
        }

        [Command("getcommits")]
        [Summary("returns some commits")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetCommits()
        {
            await _broadcastService.CheckCommits();
        }
    }
}
