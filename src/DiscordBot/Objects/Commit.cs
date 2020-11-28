using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Objects
{
    public class Commit
    {
        public int Id { get; set; }

        public string Repo { get; set; }

        public string Branch { get; set; }

        public string Changeset { get; set; }

        public User User { get; set; }
    }
}
