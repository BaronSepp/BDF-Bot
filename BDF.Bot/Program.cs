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
using Lavalink4NET.MemoryCache;
using System.Net.WebSockets;
using Lavalink4NET.Tracking;

namespace BDF.Bot
{
    internal static class Program
    {
        public static bool AudioEnabled = true;

        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var audio = services.GetRequiredService<IAudioService>();
            var tracking = services.GetRequiredService<InactivityTrackingService>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            // Load token from env
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await client.SetGameAsync("?help");

            // Start Clients
            await client.StartAsync();

            try
            {
                await audio.InitializeAsync();
                tracking.BeginTracking();
            }
            catch (WebSocketException)
            {
                var log = new LogMessage(LogSeverity.Error, "Lavalink", "Server unreachable!", null);
                await LogAsync(log);
                await client.SetGameAsync("Audio Disabled");
                AudioEnabled = false;
            }

            // Register commands
            await services.GetRequiredService<CommandHandleService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {

            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandleService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton(new LavalinkNodeOptions
                {
                    RestUri = "http://localhost:2333/",
                    WebSocketUri = "ws://localhost:2333/",
                    Password = "youshallnotpass",
                    DisconnectOnStop = false,
                    ReconnectStrategy = ReconnectStrategies.DefaultStrategy,
                    AllowResuming = true,
                    BufferSize = 1024 * 1024 * 512
                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton(new InactivityTrackingOptions
                {
                    PollInterval = TimeSpan.FromSeconds(15),
                })
                .AddSingleton<InactivityTrackingService>()
                .BuildServiceProvider();
        }
    }
}
