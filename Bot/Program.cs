using Bot.Handlers;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.MemoryCache;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;

namespace Bot
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            await host.RunAsync();

            return 0;
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(c =>
                    {
                        c.UseUtcTimestamp = true;
                        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                        c.IncludeScopes = false;
                        c.ColorBehavior = LoggerColorBehavior.Enabled;
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // HttpClientFactory
                    services.AddHttpClient();

                    // Wrapper
                    services.AddSingleton<LoggingHandler>();
                    services.AddHostedService<DiscordService>();

                    // Discord
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<CommandHandler>();
                    services.AddSingleton<PictureHandler>();
                    services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();

                    // Lavalink
                    services.AddSingleton<IAudioService, LavalinkNode>();
                    services.AddSingleton(new LavalinkNodeOptions
                    {
                        RestUri = "http://localhost:2333/",
                        WebSocketUri = "ws://localhost:2333/",
                        Password = "youshallnotpass",
                        DisconnectOnStop = false,
                        ReconnectStrategy = ReconnectStrategies.DefaultStrategy,
                        AllowResuming = true,
                        BufferSize = 1024 * 1024 * 512
                    });
                    services.AddSingleton<ILavalinkCache, LavalinkCache>();

                    // Inactivity
                    services.AddSingleton(new InactivityTrackingOptions
                    {
                        PollInterval = TimeSpan.FromSeconds(15),
                    });
                    services.AddSingleton<InactivityTrackingService>();

                })
                .UseConsoleLifetime();
        }
    }
}
