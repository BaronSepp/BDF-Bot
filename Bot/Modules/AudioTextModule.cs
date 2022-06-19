using Discord;
using Discord.Commands;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Modules;

[Name("Music")]
[RequireContext(ContextType.Guild)]
public class AudioTextModule : ModuleBase<SocketCommandContext>
{
	private readonly IAudioService _audioService;

	public AudioTextModule(IAudioService audioService, InactivityTrackingService trackingService)
	{
		_audioService = audioService;
		trackingService.InactivePlayer += DisconnectEvent;
	}

	[Command("connect", RunMode = RunMode.Async)]
	[Alias("join")]
	public async Task ConnectAsync()
	{
		await GetPlayerAsync();
	}

	[Command("disconnect", RunMode = RunMode.Async)]
	[Alias("leave")]
	public async Task DisconnectAsync()
	{
		if (IsValidUser() is false)
		{
			return;
		}

		var player = await GetPlayerAsync();
		await DisconnectEvent(this, new InactivePlayerEventArgs(_audioService, player));
	}

	[Command("play", RunMode = RunMode.Async)]
	public async Task PlayAsync([Remainder] string query)
	{
		var player = await GetPlayerAsync();
		if (player is null)
		{
			return;
		}

		var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);
		if (track is null)
		{
			await ReplyAsync(Format.Bold("No results."));
			return;
		}

		var position = await player.PlayAsync(track, true, null, null, false);
		if (position is 0)
		{
			await ReplyAsync($"{Format.Bold("Playing: ")}" + track.Title);
		}
		else
		{
			await ReplyAsync($"{Format.Bold("Added to queue: ")}" + track.Title);
		}
	}

	[Command("stop", RunMode = RunMode.Async)]
	public async Task StopAsync()
	{
		if (IsValidUser() is false)
		{
			return;
		}

		var player = await GetPlayerAsync();
		if (player is null)
		{
			return;
		}

		if (player.CurrentTrack is null)
		{
			await ReplyAsync("Nothing playing!");
			return;
		}

		await player.StopAsync(false);
		await ReplyAsync("Stopped playing.");
	}

	[Command("position", RunMode = RunMode.Async)]
	[Alias("pos")]
	public async Task PositionAsync()
	{
		var player = await GetPlayerAsync();
		if (player is null)
		{
			return;
		}

		if (player.CurrentTrack is null)
		{
			await ReplyAsync("Nothing playing!");
			return;
		}
		await ReplyAsync($"{Format.Bold("Position: ")}{player.Position:hh:mm:ss} / {player.CurrentTrack.Duration:hh:mm:ss}.");
	}

	[Command("skip", RunMode = RunMode.Async)]
	[Alias("next")]
	public async Task SkipAsync()
	{
		if (IsValidUser() is false)
		{
			return;
		}

		var player = await GetPlayerAsync();
		if (player is null)
		{
			return;
		}

		if (player.CurrentTrack is null)
		{
			await ReplyAsync("Nothing playing!");
			return;
		}

		await player.SkipAsync();
	}

	[Command("queue", RunMode = RunMode.Async)]
	[Alias("list")]
	public async Task QueueAsync()
	{
		var player = await GetPlayerAsync();
		if (await GetPlayerAsync() is null)
		{
			return;
		}

		if (player.Queue == null || player.Queue.Tracks.Count == 0)
		{
			await ReplyAsync("Nothing in Queue!");
			return;
		}

		var i = 1;
		var tracks = new List<string>(player.Queue.Count);
		foreach (var song in player.Queue.Tracks)
		{
			tracks.Add($"{i}. {song.Title}");
			i++;
		}
		await ReplyAsync(string.Join("\n", tracks));
	}

	[Command("volume", RunMode = RunMode.Async)]
	public async Task VolumeAsync(int volume = 50)
	{
		if (IsValidUser() is false)
		{
			return;
		}

		if (volume > 150 || volume < 0)
		{
			await ReplyAsync("Volume out of range: 0% - 150%!");
			return;
		}

		var player = await GetPlayerAsync();
		if (player is null)
		{
			return;
		}

		await player.SetVolumeAsync(volume / 100f);
		await ReplyAsync($"Volume updated: {volume}%");
	}

	private async Task<VoteLavalinkPlayer> GetPlayerAsync()
	{
		var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild);
		if (player is not null && player.State is not PlayerState.NotConnected or PlayerState.Destroyed)
		{
			return player;
		}

		var user = Context.Guild.GetUser(Context.User.Id);
		if (user.VoiceState.HasValue is false)
		{
			await ReplyAsync("You must be in a voice channel!");
			return null;
		}

		return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.VoiceChannel, true, false);
	}

	private async Task DisconnectEvent(object _, InactivePlayerEventArgs eventArgs)
	{
		if (eventArgs.Player is null || eventArgs.Player.State is PlayerState.Destroyed or PlayerState.NotConnected) return;

		await eventArgs.Player.DisconnectAsync();
		await eventArgs.Player.DisposeAsync();
	}

	private bool IsValidUser()
	{
		var user = Context.Guild.GetUser(Context.User.Id);
		var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild);

		if (user is null || user.VoiceChannel is null)
		{
			return false;
		}

		if (user.VoiceChannel.Id != player.VoiceChannelId)
		{
			return false;
		}

		return true;
	}
}