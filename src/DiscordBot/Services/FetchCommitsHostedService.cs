using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

#pragma warning disable 4014

namespace DiscordBot.Services
{
    public class FetchCommitsHostedService: BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IBroadcastService _broadcastService;

        public FetchCommitsHostedService(ILoggerFactory logger, IBroadcastService broadcastService)
        {
            _logger = logger.CreateLogger("FetchCommitsHostedService");
            _broadcastService = broadcastService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {

                await RefreshCommitsLog(cancellationToken);
            }

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
                await Task.Delay(5000);
            }
        }

    }
}