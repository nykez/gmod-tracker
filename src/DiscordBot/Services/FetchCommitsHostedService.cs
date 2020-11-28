using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

#pragma warning disable 4014

namespace DiscordBot.Services
{
    public class FetchCommitsHostedService: IHostedService
    {
        private readonly ILogger _logger;
        private readonly IBroadcastService _broadcastService;

        public FetchCommitsHostedService(ILoggerFactory logger, IBroadcastService broadcastService)
        {
            _logger = logger.CreateLogger("FetchCommitsHostedService");
            _broadcastService = broadcastService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {jobName}", nameof(FetchCommitsHostedService));
            Console.WriteLine($"Starting {nameof(FetchCommitsHostedService)}");

            RefreshCommitsLog(cancellationToken);

            return Task.CompletedTask;
        }

        private async Task RefreshCommitsLog(CancellationToken stoppingToken)
        {
            Console.WriteLine("doing this!");
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    await _broadcastService.CheckCommits();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Job {jobName} threw an exception", nameof(FetchCommitsHostedService));
                }

                // delay 5 minutes
                await Task.Delay(5000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping {jobName}", nameof(FetchCommitsHostedService));

            return Task.CompletedTask;

        }
    }
}