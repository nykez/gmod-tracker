using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Services
{
    public class FetchCommitsHostedService: IHostedService
    {
        private readonly ILogger<FetchCommitsHostedService> _logger;
        private readonly IBroadcastService _broadcastService;

        public FetchCommitsHostedService(ILogger<FetchCommitsHostedService> logger, IBroadcastService broadcastService)
        {
            _logger = logger;
            _broadcastService = broadcastService;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {jobName}", nameof(FetchCommitsHostedService));
            Console.WriteLine($"Starting {nameof(FetchCommitsHostedService)}");

            return Task.CompletedTask;
        }

        private async Task RefreshCommitsLog(CancellationToken stoppingToken)
        {
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
                await Task.Delay(300000);
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping {jobName}", nameof(FetchCommitsHostedService));

            return Task.CompletedTask;

        }
    }
}