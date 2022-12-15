using Discord;
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;

namespace Bot.Modules;

[RequireContext(ContextType.Guild)]
[Group("audio", "Contains audio related commands.")]
public sealed class AudioModule : InteractionModuleBase<SocketInteractionContext>
{
	private readonly IAudioService _audioService;

	public AudioModule(IAudioService audioService, InactivityTrackingService trackingService)
	{
		_audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
		trackingService.InactivePlayer += DisconnectEvent;
	}

	[SlashCommand("help", "Shows available audio commands.")]
	public async Task HelpAudioAsync()
	{
		await RespondAsync(
			$"{Format.Bold("Available Audio Commands:")}{Environment.NewLine}" +
			$"- /audio play {Format.Code("url")}: Plays song from given link or query.{Environment.NewLine}" +
			$"- /audio skip: Skips the current song in queue and plays the next.{Environment.NewLine}" +
			$"- /audio queue: Lists all songs in queue.{Environment.NewLine}" +
			$"- /audio stop: Stops playback.{Environment.NewLine}" +
			$"- /audio volume {Format.Code("percentage")}: Sets the playback volume (0 - 150).");
	}

	[SlashCommand("play", "Plays song from given link or query.", runMode: RunMode.Async)]
	public async Task PlayAsync(string url)
	{
		await DeferAsync();
		var player = await GetPlayerAsync();

		var track = await _audioService.GetTrackAsync(url, SearchMode.YouTube);
		if (track is null)
		{
			await FollowupAsync(Format.Bold("No results."));
			return;
		}

		var position = await player.PlayAsync(track, true, null, null, false);
		if (position is 0)
		{
			await FollowupAsync($"{Format.Bold("Playing: ")}" + track.Title);
		}
		else
		{
			await FollowupAsync($"{Format.Bold("Added to queue: ")}" + track.Title);
		}
	}

	[SlashCommand("stop", "Stops playback.", runMode: RunMode.Async)]
	public async Task StopAsync()
	{
		await DeferAsync();

		if (await IsValidUserAsync() is false)
		{
			return;
		}

		var player = await GetPlayerAsync();
		if (player.CurrentTrack is null)
		{
			await FollowupAsync("Nothing playing!");
			return;
		}

		await player.StopAsync(false);
		await FollowupAsync("Stopped playing.");
	}

	[SlashCommand("skip", "Skips the current song in queue and plays the next.", runMode: RunMode.Async)]
	public async Task SkipAsync()
	{
		await DeferAsync();

		if (await IsValidUserAsync() is false)
		{
			return;
		}

		var player = await GetPlayerAsync();
		if (player.CurrentTrack is null)
		{
			await FollowupAsync("Nothing playing!");
			return;
		}

		await player.SkipAsync();
		await FollowupAsync("Skipped track.");
	}

	[SlashCommand("queue", "Lists all songs in queue.", runMode: RunMode.Async)]
	public async Task QueueAsync()
	{
		await DeferAsync(true);

		var player = await GetPlayerAsync();
		if (player.Queue is null || player.Queue.Tracks.Count is 0)
		{
			await FollowupAsync("Nothing in Queue!");
			return;
		}

		var i = 1;
		var tracks = new List<string>(player.Queue.Count);
		foreach (var song in player.Queue.Tracks)
		{
			tracks.Add($"{i}. {song.Title}");
			i++;
		}
		await FollowupAsync(string.Join("\n", tracks));
	}

	[SlashCommand("volume", "Sets the playback volume", runMode: RunMode.Async)]
	public async Task VolumeAsync(int volume)
	{
		await DeferAsync();

		if (await IsValidUserAsync() is false)
		{
			return;
		}

		if (volume > 150 || volume < 0)
		{
			await FollowupAsync("Volume out of range: 0% - 150%!");
			return;
		}

		var player = await GetPlayerAsync();

		await player.SetVolumeAsync(volume / 100f);
		await FollowupAsync($"Volume updated: {volume}%");
	}

	private async Task<VoteLavalinkPlayer> GetPlayerAsync()
	{
		var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild);
		if (player is not null && player.State is not PlayerState.NotConnected or PlayerState.Destroyed)
		{
			return player;
		}

		var user = Context.Guild.GetUser(Context.User.Id);
		return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.VoiceChannel, true, false);
	}

	private async Task DisconnectEvent(object _, InactivePlayerEventArgs eventArgs)
	{
		await eventArgs.Player.DisconnectAsync();
		await eventArgs.Player.DisposeAsync();
	}

	private async Task<bool> IsValidUserAsync()
	{
		var user = Context.Guild.GetUser(Context.User.Id);
		var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild);

		if (user is null || user.VoiceChannel is null)
		{
			await FollowupAsync("You cannot use this command.");
			return false;
		}

		if (user.VoiceChannel.Id != player.VoiceChannelId)
		{
			await FollowupAsync("You are not in the correct voice channel.");
			return false;
		}

		return true;
	}
}