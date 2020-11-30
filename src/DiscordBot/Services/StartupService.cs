using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using System.Threading;

namespace DiscordBot.Services
{
    public class StartupService : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config)
        {
            _provider = provider;
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            string discordToken = _config["token"];     // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");

            // set status
            await _discord.SetGameAsync("Watching for commits...");
            Console.WriteLine($"Total Guilds: {_discord.Guilds}");

        }

    }
}
