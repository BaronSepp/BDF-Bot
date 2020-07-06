using System;
using System.Net.Http;
using System.Threading.Tasks;
using BDF.Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Microsoft.Extensions.DependencyInjection;
using Lavalink4NET.Logging;
using Lavalink4NET.MemoryCache;

namespace BDF.Bot
{
    class Program
    {
        static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var audio = services.GetRequiredService<IAudioService>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            // Load token from env
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await client.SetGameAsync("!help or ?help");

            // Start Clients
            await client.StartAsync();
            await audio.InitializeAsync();

            // Register commands
            await services.GetRequiredService<CommandHandleService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandleService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton(new LavalinkNodeOptions
                {
                    RestUri = "http://localhost:2333/",
                    WebSocketUri = "ws://localhost:2333/",
                    Password = "youshallnotpass",
                    DisconnectOnStop = false,
                    ReconnectStrategy = ReconnectStrategies.DefaultStrategy,
                    AllowResuming = true,
                    BufferSize = 1024 * 1024 * 512 // 1 MiB
                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .BuildServiceProvider();
        }
    }
}
