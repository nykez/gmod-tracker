using System;
using System.Collections.Generic;
using System.Text;

using DiscordBot.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Context
{
    public partial class BotContext: DbContext
    {

        public virtual DbSet<RegisteredChannel> Channels { get; set; }
        public virtual DbSet<LastestCommit> LastCommit { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = "discordbot.db"};
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }

    }

}
