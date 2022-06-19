using Bot.Handlers;
using Bot.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.MemoryCache;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bot;

public static class Program
{

	private static async Task<int> Main(string[] args)
	{
		using var host = CreateHostBuilder(args).Build();
		await host.RunAsync();

		return Environment.ExitCode;
	}

	private static IHostBuilder CreateHostBuilder(string[] args)
	{
		return Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
			{
				logging.AddSimpleConsole(c =>
				{
					c.UseUtcTimestamp = true;
					c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
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
				services.AddSingleton<InteractionService>();
				services.AddSingleton<CommandHandler>();
				services.AddSingleton<PictureHandler>();
				services.AddSingleton<InteractionHandler>();
				services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
				services.AddSingleton(new DiscordSocketConfig
				{
					GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
					AlwaysDownloadUsers = true
				});

				// Lavalink
				services.AddSingleton<IAudioService, LavalinkNode>();
				services.AddSingleton(new LavalinkNodeOptions
				{
					RestUri = $"http://{hostContext.Configuration["LavaUri"]}/",
					WebSocketUri = $"ws://{hostContext.Configuration["LavaUri"]}/",
					Password = "youshallnotpass",
					DisconnectOnStop = false,
					ReconnectStrategy = ReconnectStrategies.DefaultStrategy,
					AllowResuming = true,
					BufferSize = 1024 * 512
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