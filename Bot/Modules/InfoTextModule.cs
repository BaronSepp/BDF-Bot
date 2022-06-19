using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Modules;

public class InfoTextModule : ModuleBase<SocketCommandContext>
{
	[Command("info")]
	[Alias("about")]
	public async Task InfoAsync()
	{
		var app = await Context.Client.GetApplicationInfoAsync();

		await ReplyAsync(
			$"- Author: {app.Owner}{Environment.NewLine}" +
			$"- Uptime: {GetUptime()}{Environment.NewLine}" +
			$"- Guilds: {Context.Client.Guilds.Count}{Environment.NewLine}" +
			$"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}{Environment.NewLine}" +
			$"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}{Environment.NewLine}");
	}

	[Command("help")]
	[Alias("commands")]
	public async Task HelpAsync()
	{
		await ReplyAsync(
			$"{Format.Bold("Available Commands:")}{Environment.NewLine}" +
			$"- ?info: Displays bot info.{Environment.NewLine}" +
			$"- ?help: Lists available commands.{Environment.NewLine}" +
			$"- ?audio: Lists audio commands.{Environment.NewLine}" +
			$"- ?image: Lists image commands.{Environment.NewLine}" +
			$"- ?echo {Format.Code("msg")}: Makes the bot say something.{Environment.NewLine}" +
			$"- ?ping: Test latency.{Environment.NewLine}" +
			$"- ?clean {Format.Code("amount")}: Deletes the specified amount of messages.");
	}

	[Command("audio")]
	[Alias("help audio", "sound", "help sound")]
	public async Task HelpAudioAsync()
	{
		await ReplyAsync(
			$"{Format.Bold("Available Music Commands:")}{Environment.NewLine}" +
			$"- ?play {Format.Code("url")}: Plays song from given link.{Environment.NewLine}" +
			$"- ?disconnect: Disconnects the bot from the voice channel.{Environment.NewLine}" +
			$"- ?connect: Connects the bot to the voice channel.{Environment.NewLine}" +
			$"- ?position: Shows elapsed track time.{Environment.NewLine}" +
			$"- ?skip: Skips the current song in queue and plays the next.{Environment.NewLine}" +
			$"- ?queue: Lists all songs in queue.{Environment.NewLine}" +
			$"- ?stop: Stops playback.{Environment.NewLine}" +
			$"- ?volume {Format.Code("percentage")}: Sets the playback volume (0 - 150).");
	}

	[Command("image")]
	[Alias("help image", "picture", "help picture")]
	public async Task HelpImageAsync()
	{
		await ReplyAsync(
			$"{Format.Bold("Available Image Commands:")}{Environment.NewLine}" +
			$"- ?r34 {Format.Code("query")}: Returns a r34 image based on query.{Environment.NewLine}" +
			$"- ?anime {Format.Code("query")}: Returns a anime image based on query.{Environment.NewLine}" +
			 "- ?cat: Returns cat picture.");
	}

	private static string GetUptime()
	{
		return (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss", Constants.Culture);
	}
}