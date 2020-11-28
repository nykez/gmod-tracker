using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Objects
{
    public class Result
    {
        public int total { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public Commit[] results { get; set; }
    }
}