using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.Models
{
    public class RegisteredChannel
    {
        [Key]
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong GuildId { get; set; }
    }

}
