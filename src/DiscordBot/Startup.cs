using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Services;
using DiscordBot.Context;
using System.Net.Http;

namespace DiscordBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration{ get; set; }

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json");
            Configuration = builder.Build();
        }
        
        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();             // Create a new instance of a service collection
            ConfigureServices(services);

            var provider = services.BuildServiceProvider(); // build provider
            provider.GetRequiredService<LoggingService>(); // start logging service
            provider.GetRequiredService<CommandHandlingService>(); // start command handling service
            provider.GetRequiredService<FetchCommitsHostedService>();

            await provider.GetRequiredService<StartupService>().StartAsync();

            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose, // verbose info
                MessageCacheSize = 300 // 300 messages per channel
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
            }))
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<StartupService>()
            .AddSingleton<LoggingService>()
            .AddDbContext<BotContext>()
            .AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://commits.facepunch.com") })
            .AddSingleton<IBroadcastService, BroadcastService>()
            .AddHostedService<FetchCommitsHostedService>()
            .AddSingleton(Configuration);

        }
    }
}
