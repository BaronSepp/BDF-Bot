using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot.Handlers;
public class InteractionHandler
{
	private readonly IConfiguration _configuration;
	private readonly IServiceProvider _serviceProvider;
	private readonly IHostEnvironment _hostEnvironment;
	private readonly DiscordSocketClient _discordSocketClient;
	private readonly InteractionService _interactionService;

	public InteractionHandler(
		IConfiguration configuration,
		IServiceProvider serviceProvider,
		IHostEnvironment hostEnvironment,
		DiscordSocketClient discordSocketClient,
		InteractionService interactionService)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
		_hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment)); ;
		_discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient)); ;
		_interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService)); ;
	}

	public async Task InitializeAsync()
	{
		// Process the Interaction related payloads to execute commands
		_discordSocketClient.InteractionCreated += InteractionCreatedAsync;
		_discordSocketClient.ButtonExecuted += ButtonExecutedAsync;

		// Context & Slash commands can be automatically registered,
		// but this process needs to happen after the client enters the READY state.
		_discordSocketClient.Ready += ReadyAsync;

		// Register modules that are public and inherit InteractionModuleBase<T>.
		await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
	}

	private async Task ReadyAsync()
	{
		// Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
		object _ = _hostEnvironment.IsDevelopment()
			? await _interactionService.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("TestGuildId"), true)
			: await _interactionService.RegisterCommandsGloballyAsync(true);
	}

	private async Task InteractionCreatedAsync(SocketInteraction socketInteraction)
	{
		try
		{
			// Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
			var context = new SocketInteractionContext(_discordSocketClient, socketInteraction);

			// Execute the incoming command.
			var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

			if (result.IsSuccess is false)
			{
				switch (result.Error)
				{
					case InteractionCommandError.UnmetPrecondition:
						// implement
						break;
					default:
						break;
				}
			}
		}
		catch
		{
			// If Slash Command execution fails it is most likely that the original interaction acknowledgment will persist. It is a good idea to delete the original
			// response, or at least let the user know that something went wrong during the command execution.
			if (socketInteraction.Type is InteractionType.ApplicationCommand)
			{
				var response = await socketInteraction.GetOriginalResponseAsync();
				await response.DeleteAsync();
			}
		}
	}
	private async Task ButtonExecutedAsync(SocketMessageComponent socketInteraction)
	{
		var context = new SocketInteractionContext(_discordSocketClient, socketInteraction);
		await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
	}
}
