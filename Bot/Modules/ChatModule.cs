using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.Modules;
public sealed class ChatModule : InteractionModuleBase<SocketInteractionContext>
{
	private readonly IHostApplicationLifetime _hostApplicationLifetime;

	public ChatModule(IHostApplicationLifetime hostApplicationLifetime)
	{
		_hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
	}

	[SlashCommand("prune", "Deletes the specified amount of messages.", runMode: RunMode.Async)]
	[RequireUserPermission(GuildPermission.ManageMessages)]
	[RequireBotPermission(ChannelPermission.ManageMessages)]
	public async Task PruneMessagesAsync(int amount)
	{
		var messages = Context.Channel.GetMessagesAsync(amount).Flatten();
		var filteredMessages = new List<IMessage>(100);

		await foreach (var message in messages.WithCancellation(_hostApplicationLifetime.ApplicationStopping))
		{
			if (message.Timestamp > DateTimeOffset.Now.AddDays(-14))
			{
				filteredMessages.Add(message);
			}
		}

		await ((SocketTextChannel)Context.Channel)?.DeleteMessagesAsync(filteredMessages);
		await RespondAsync($"Deleted {filteredMessages.Count} messages.", ephemeral: true);
	}

	[SlashCommand("ping", "Pings the bot and returns its latency.", runMode: RunMode.Sync)]
	public async Task Ping()
	{
		await RespondAsync($"Pong! {Context.Client.Latency}ms", ephemeral: true);
	}
}
