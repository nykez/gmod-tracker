using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.Models
{
    public class RegisteredChannel
    {
        public RegisteredChannel() { }

        public RegisteredChannel(ulong channelID, ulong guildID)
        {
            ChannelId = channelID;
            GuildId = guildID;
        }

        [Key]
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong GuildId { get; set; }
    }

}
