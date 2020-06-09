using BDF.Bot.Services;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace BDF.Bot.Modules
{
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
        [Alias("rule34")]
        public async Task Rule34(string text)
        {
            var stream = await PictureService.GetRule34(text);
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "rule34.jpg");
        }

        [Command("anime")]
        public async Task Anime(string text)
        {
            var stream = await PictureService.GetAnime(text);
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "rule34.jpg");
        }
    }

}
