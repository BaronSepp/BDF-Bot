using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BDF.Bot.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;

        public PictureService(HttpClient http)
            => _http = http;

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetKjentiAsync()
        {
            var resp = await _http.GetAsync("https://i.imgur.com/fHLpyAQ.jpg");
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}
