using System;
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
        // Dependency Injection will fill this value in for us

        public PublicModule(BotContext context)
        {
            _context = context;
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

        [Command("setchannel")]
        [Summary("Sets the channel to broadcast commits to.")]
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
                    $"{MentionUtils.MentionUser(Context.User.Id)} Guild already has a channel set for commits.");
                return;
            }
                

            await _context.Channels.AddAsync(new RegisteredChannel {ChannelId = channel.Id, GuildId = Context.Guild.Id});
            await _context.SaveChangesAsync();
            
            // we have a found channel: process it and save it into database to broadcast too
            // if we broadcast to the channel, and it's not valid then remove it from the database
            // and alert an admin?

            await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} set Garry's Mod commit channel to {MentionUtils.MentionChannel(channel.Id)}!");
        }
    }
}
