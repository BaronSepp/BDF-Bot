using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BDF.Bot.Services
{
    public class PictureService
    {
        public async Task<Stream> GetCatPictureAsync()
        {
            using var client = new HttpClient();
            {
                var resp = await client.GetAsync("https://cataas.com/cat");
                return await resp.Content.ReadAsStreamAsync();
            }
        }

        public async Task<Stream> GetRule34(string[] query)
        {
            var r34 = new BooruSharp.Booru.Rule34();
            var result = await r34.GetRandomPostAsync(query);

            using var client = new HttpClient();
            {
                var resp = await client.GetAsync(result.FileUrl.AbsoluteUri);
                return await resp.Content.ReadAsStreamAsync();
            }
        }

        public async Task<Stream> GetAnime(string[] query)
        {
            var safe = new BooruSharp.Booru.Safebooru();
            var result = await safe.GetRandomPostAsync(query);
            using var client = new HttpClient();
            {
                var resp = await client.GetAsync(result.FileUrl.AbsoluteUri);
                return await resp.Content.ReadAsStreamAsync();
            }
        }
    }
}
