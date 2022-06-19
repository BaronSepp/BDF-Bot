using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot.Handlers;

public class CommandHandler
{
	// Fields
	private readonly CommandService _commandService;
	private readonly DiscordSocketClient _discordSocketClient;
	private readonly IServiceProvider _serviceProvider;

	public CommandHandler(IServiceProvider serviceProvider, CommandService commandService, DiscordSocketClient discordSocketClient)
	{
		// DI
		_commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
		_discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient)); ;
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
	}

	public async Task InitializeAsync()
	{
		// Hook CommandExecuted to handle post-command-execution logic.
		_commandService.CommandExecuted += CommandExecutedAsync;

		// Hook MessageReceived so we can process each message to see
		_discordSocketClient.MessageReceived += MessageReceivedAsync;

		// Register modules that are public and inherit ModuleBase<T>.
		await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
	}

	private async Task MessageReceivedAsync(SocketMessage rawMessage)
	{
		// Ignore system messages, or messages from other bots
		if (rawMessage is not SocketUserMessage message)
		{
			return;
		}

		if (message.Source is not MessageSource.User)
		{
			return;
		}

		// This value holds the offset where the prefix ends
		var argPos = 0;
		// Perform prefix check.
		if (!message.HasCharPrefix('?', ref argPos))
		{
			return;
		}

		var context = new SocketCommandContext(_discordSocketClient, message);
		// Perform the execution of the command. In this method,
		// the command service will perform precondition and parsing check
		// then execute the command if one is matched.
		await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
		// Note that normally a result will be returned by this format, but here
		// we will handle the result in CommandExecutedAsync,
	}

	private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
	{
		// command is unspecified when there was a search failure (command not found); we don't care about these errors
		if (!command.IsSpecified)
		{
			return;
		}

		// the command was successful, we don't care about this result, unless we want to log that a command succeeded.
		if (result.IsSuccess)
		{
			return;
		}

		// the command failed, let's notify the user that something happened.
		await context.Channel.SendMessageAsync("Oops.. Something went wrong :(");
	}
}