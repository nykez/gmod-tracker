using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            // build our config (bot socket/prefix as of 11/27/2020)
            _config = BuildConfig();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();

            // auth to discord
            await _client.LoginAsync(TokenType.Bot, _config["token"]);

            // start bot
            await _client.StartAsync();

            // set status
            await _client.SetGameAsync("Watching for commits...");

            _client.Ready += _client_Ready;

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();


            await Task.Delay(-1);
        }

        private async Task _client_Ready()
        {
            
            await Task.Run(() => Console.WriteLine($"Total Guilds: {_client.Guilds.Count}"));
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddDbContext<BotContext>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(_config)

                // Add additional services here...
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json")
                .Build();
        }

    }
}