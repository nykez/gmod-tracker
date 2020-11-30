using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Models
{
    public class LastestCommit
    {
        [Key]
        public int LastCommit { get; set; }
    }
}
