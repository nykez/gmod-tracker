using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using DiscordBot.Context;
using DiscordBot.Services;
using System.Net.Http;
using System;

namespace DiscordBot
{
    class Program
    {
        public IConfigurationRoot Configuration { get; set; }

        static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    //See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/ for configuration source options
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json")
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
               {
                   //The default console logger doesn't have a great format, I recommend using a third-party one as is shown in the Serilog example
                   x.AddConsole();
                   x.SetMinimumLevel(LogLevel.Debug);
               })
                //Specify the type of discord.net client via the type parameter
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };

                    config.Token = context.Configuration["token"];
                })
                //Omit this if you don't use the command service
                .UseCommandService( (context, config) =>
                {
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = Discord.Commands.RunMode.Async;
                })
                .ConfigureServices((context, services) =>
                {

                    services.AddHostedService<CommandHandlingService>();
                    services.AddHostedService<StartupService>();
                    services.AddDbContext<BotContext>();
                    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://commits.facepunch.com") });
                    services.AddSingleton<IBroadcastService, BroadcastService>();
                    //services.AddHostedService<FetchCommitsHostedService>();
                })
                .UseConsoleLifetime();


            var host = builder.Build();

            using (host)
            {
                //Fire and forget. Will run until console is closed or the service is stopped. Basically the same as normally running the bot.
                await host.RunAsync();
            }
        }
    }

}