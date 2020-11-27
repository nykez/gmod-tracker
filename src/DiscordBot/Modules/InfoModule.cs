﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Name("General Commands")]
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

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

            var rolesField = new EmbedFieldBuilder();
            rolesField.Name = "Roles";
            foreach (var role in ((SocketGuildUser) user).Roles)
            {
                rolesField.Value += $"{role.Name}\n";
            }

            var createdAtField = new EmbedFieldBuilder()
            {
                Name = "Created At",
                Value = user.CreatedAt.DateTime
            };

            var statusField = new EmbedFieldBuilder()
            {
                Name = "Status",
                Value = user.Status
            };


            embed.AddField(rolesField);
            embed.AddField(createdAtField);
            embed.AddField(statusField);

            

            await ReplyAsync(embed: embed.Build());
        }

    }
}