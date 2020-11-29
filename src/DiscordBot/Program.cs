using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Context;



namespace DiscordBot
{
    class Program
    {
        public static Task Main(string[] args)
            => Startup.RunAsync(args);
    }

}