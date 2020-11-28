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
            throw new NotImplementedException();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}