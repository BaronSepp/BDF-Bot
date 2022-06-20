using Bot.Handlers;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Modules;

[Group("image", "All image related commands")]
public class ImageModule : InteractionModuleBase<SocketInteractionContext>
{
	private readonly PictureHandler _pictureHandler;
	private readonly ILogger<ImageModule> _logger;

	public ImageModule(PictureHandler pictureHandler, ILogger<ImageModule> logger)
	{
		_pictureHandler = pictureHandler ?? throw new ArgumentNullException(nameof(pictureHandler));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	[SlashCommand("cat", "Shows a random image of a cat.")]
	public async Task CatAsync()
	{
		var stream = await _pictureHandler.GetCatPictureAsync();
		stream.Seek(0, SeekOrigin.Begin);
		await Context.Channel.SendFileAsync(stream, "cat.png");
	}

	[SlashCommand("r34", "Returns a random image that matches the given query.")]
	[RequireNsfw]
	public async Task Rule34Async(string query = "")
	{
		try
		{
			await DeferAsync();
			var queryArray = query.Split(' ');
			var stream = await _pictureHandler.GetRule34(queryArray);
			stream.Seek(0, SeekOrigin.Begin);
			await FollowupWithFileAsync(stream, "rule34.png");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred fetching Rule34 image!");
			await FollowupAsync("Nothing found.", ephemeral: true);
		}

	}

	[SlashCommand("anime", "Returns a random image that matches the given query.")]
	public async Task AnimeAsync(string query = "")
	{
		try
		{
			await DeferAsync();
			var queryArray = query.Split(' ');
			var stream = await _pictureHandler.GetAnime(queryArray);
			stream.Seek(0, SeekOrigin.Begin);
			await FollowupWithFileAsync(stream, "anime.png");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred fetching Anime image!");
			await FollowupAsync("Nothing found.", ephemeral: true);
		}
	}
}