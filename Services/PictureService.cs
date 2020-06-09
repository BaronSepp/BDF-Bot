using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BDF.Bot.Services
{
    public class PictureService
    {
        private readonly HttpClient http;

        public PictureService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetRule34(string query)
        {
            BooruSharp.Booru.Rule34 booru = new BooruSharp.Booru.Rule34();
            BooruSharp.Search.Post.SearchResult result = await booru.GetRandomImageAsync(query);
            var resp = await http.GetAsync(result.fileUrl.AbsoluteUri);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetAnime(string query)
        {
            BooruSharp.Booru.Safebooru booru = new BooruSharp.Booru.Safebooru();
            BooruSharp.Search.Post.SearchResult result = await booru.GetRandomImageAsync(query);
            var resp = await http.GetAsync(result.fileUrl.AbsoluteUri);
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}
