using Discord;
using Discord.Interactions;
using System.Diagnostics;

namespace Bot.Modules;

public sealed class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("info", "Shows bot statistics.")]
	public async Task InfoAsync()
	{
		var app = await Context.Client.GetApplicationInfoAsync();

		await RespondAsync(
			$"- Author: {app.Owner}{Environment.NewLine}" +
			$"- Uptime: {GetUptime()}{Environment.NewLine}" +
			$"- Guilds: {Context.Client.Guilds.Count}{Environment.NewLine}" +
			$"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}{Environment.NewLine}" +
			$"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}{Environment.NewLine}");
	}

	[SlashCommand("help", "Shows available help commands.")]
	public async Task HelpAsync()
	{
		await RespondAsync(
			$"{Format.Bold("Available Commands:")}{Environment.NewLine}" +
			$"- /info: Displays bot info.{Environment.NewLine}" +
			$"- /help: Lists available commands.{Environment.NewLine}" +
			$"- /help audio: Lists audio commands.{Environment.NewLine}" +
			$"- /help image: Lists image commands.{Environment.NewLine}" +
			$"- /ping: Pings the bot and returns its latency.{Environment.NewLine}" +
			$"- /prune {Format.Code("amount")}: Deletes the specified amount of messages.");
	}

	private static string GetUptime()
	{
		return (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss", Constants.Culture);
	}
}