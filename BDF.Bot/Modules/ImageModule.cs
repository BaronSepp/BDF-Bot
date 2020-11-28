using BDF.Bot.Services;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace BDF.Bot.Modules
{
    [Name("Image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("cat")]
        public async Task CatAsync()
        {
            var stream = await PictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("r34")]
        [Alias("rule34", "34")]
        [RequireNsfw]
        public async Task Rule34([Remainder] string text)
        {
            try
            {
                var textArray = text.Split(' ');
                var stream = await PictureService.GetRule34(textArray);
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "rule34.png");
            }
            catch
            {
                await Context.Channel.SendMessageAsync("Nothing found.");
            }

        }

        [Command("anime")]
        [Alias("safe")]
        public async Task Anime([Remainder] string text)
        {
            try
            {
                var textArray = text.Split(' ');
                var stream = await PictureService.GetAnime(textArray);
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "anime.png");
            }
            catch
            {
                await Context.Channel.SendMessageAsync("Nothing found.");
            }
        }
    }

}
