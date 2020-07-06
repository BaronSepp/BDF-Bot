using BDF.Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;

namespace BDF.Bot.Modules
{
    [Name("Music")]
    [RequireContext(ContextType.Guild)]
    public sealed class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioService _audioService;

        public AudioModule(IAudioService audioService)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        }

        [Command("disconnect", RunMode = RunMode.Async)]
        [Alias("leave")]
        public async Task Disconnect()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            await player.StopAsync(true);
            await ReplyAsync("Goodbye.");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string query)
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);

            if (track == null)
            {
                await ReplyAsync(Format.Bold("No results."));
                return;
            }

            var position = await player.PlayAsync(track, enqueue: true);

            if (position == 0)
            {
                await ReplyAsync($"{Format.Bold("Playing: ")}" + track.Title);
            }
            else
            {
                await ReplyAsync($"{Format.Bold("Added to queue: ")}" + track.Title);
            }
        }

        [Command("position", RunMode = RunMode.Async)]
        [Alias("pos")]
        public async Task Position()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await ReplyAsync("Nothing playing!");
                return;
            }

            await ReplyAsync($"Position: {player.TrackPosition} / {player.CurrentTrack.Duration}.");
        }

        [Command("skip", RunMode = RunMode.Async)]
        [Alias("next")]
        public async Task Skip()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await ReplyAsync("Nothing playing!");
                return;
            }


            await player.SkipAsync();
        }

        [Command("queue", RunMode = RunMode.Async)]
        [Alias("list")]
        public async Task Queue()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.Queue == null || player.Queue.Tracks.Count == 0)
            {
                await ReplyAsync("Nothing in Queue!");
                return;
            }

            int i = 1;
            List<string> tracks = new List<string>();
            foreach (var song in player.Queue.Tracks)
            {
                tracks.Add($"{i}. {song.Title}");
                i++;
            }
            await ReplyAsync(string.Join("\n", tracks));
        }


        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            if (player.CurrentTrack == null)
            {
                await ReplyAsync("Nothing playing!");
                return;
            }

            await player.StopAsync(false);
            await ReplyAsync("Stopped playing.");
        }

        [Command("volume", RunMode = RunMode.Async)]
        public async Task Volume(int volume = 100)
        {
            if (volume > 200 || volume < 0)
            {
                await ReplyAsync("Volume out of range: 0% - 1000%!");
                return;
            }

            var player = await GetPlayerAsync();

            if (player == null)
            {
                return;
            }

            await player.SetVolumeAsync(volume / 100f);
            await ReplyAsync($"Volume updated: {volume}%");
        }


        private async Task<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true)
        {
            var player = _audioService.GetPlayer<VoteLavalinkPlayer>(Context.Guild);

            if (player != null
                && player.State != PlayerState.NotConnected
                && player.State != PlayerState.Destroyed)
            {
                return player;
            }

            var user = Context.Guild.GetUser(Context.User.Id);

            if (!user.VoiceState.HasValue)
            {
                await ReplyAsync("You must be in a voice channel!");
                return null;
            }

            if (!connectToVoiceChannel)
            {
                await ReplyAsync("The bot is not in a voice channel!");
                return null;
            }

            return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.VoiceChannel);
        }
    }
}
