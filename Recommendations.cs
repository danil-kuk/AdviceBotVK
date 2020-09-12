using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdviceBotVK
{
    static class Recommendations
    {
        static HttpClient client = new HttpClient();

        public async static Task<RecommendationData> GetMovieAsync(string args = null)
        {
            var pageIndex = 1 + new Random().Next(200);
            var url = $"https://api.themoviedb.org/3/discover/movie?api_key=e8bb0c0086268a0b098bc9eab975487f&language=ru-RU&sort_by=popularity.desc&include_adult=false&page={pageIndex}&{args}";
            var response = await GetDataFromApiAsync(url);
            var index = new Random().Next(20);
            var movie = new RecommendationData
            {
                Title = response?["results"][index]["title"].ToString(),
                Description = response?["results"][index]["overview"].ToString(),
                ImageUrl = "https://image.tmdb.org/t/p/w300" + response?["results"][index]["poster_path"].ToString()
            };
            return movie;
        }

        public async static Task<RecommendationData> GetSeriesAsync(string args = null)
        {
            var url = $"https://api.reelgood.com/v3.0/content/roulette?content_kind=show&{args}&free=false&minimum_imdb=6&minimum_rt=60&nocache=true&region=us&sources=netflix%2Chulu_plus%2Camazon_prime%2Cdisney_plus%2Chbo%2Capple_tv_plus%2Cfubo_tv%2Cshowtime%2Cstarz%2Ccbs_all_access%2Cepix%2Ccrunchyroll_premium%2Cfunimation%2Camc_premiere%2Ckanopy%2Ccriterion_channel%2Cbritbox%2Cdc_universe%2Cmubi%2Ccinemax%2Cfandor%2Cacorntv%2Challmark_movies_now%2Cbet_plus%2Cshudder%2Cyoutube_premium%2Cindieflix";
            var response = await GetDataFromApiAsync(url);
            var series = new RecommendationData
            {
                Title = response?["title"].ToString(),
                Description = response?["overview"].ToString(),
                ImageUrl = "https://img.reelgood.com/content/show/" + response?["id"].ToString() + "/poster-500.jpg"
            };
            return series;
        }

        public async static Task<RecommendationData> GetBookAsync(string args = null)
        {
            var itemId = await GetRandomBookIdFromCategoryAsync(args);
            var url = $"https://mybook.ru/api/books/{itemId}";
            var response = await GetDataFromApiAsync(url);
            var book = new RecommendationData
            {
                Title = response?["name"].ToString(),
                Description = response?["annotation_plain"].ToString(),
                ImageUrl = "https://i4.mybook.io/p/500x500/" + response?["default_cover"].ToString()
            };
            return book;
        }

        private async static Task<string> GetRandomBookIdFromCategoryAsync(string args)
        {
            var offset = 10 * new Random().Next(20); // Выборка будет состоять из 200 самых популярных книг в категории
            var index = new Random().Next(10);
            var url = $"https://mybook.ru/api/books/?{args}&limit=10&o=popular&offset={offset}";
            var response = await GetDataFromApiAsync(url);
            return response?["objects"][index]["id"].ToString();
        }

        public async static Task<RecommendationData> GetGameAsync(string args = null)
        {
            var appIds = await GetRandomAppIdsFromCategoryAsync(args);
            JToken response = null;
            string appId = "";
            while (response == null || !response[appId]["success"].ToObject<bool>())
            {
                appId = appIds.GetRandomItem();
                var url = $"https://store.steampowered.com/api/appdetails?appids={appId}&l=russian";
                response = await GetDataFromApiAsync(url);
            }
            var game = new RecommendationData
            {
                Title = response?[appId]["data"]["name"].ToString(),
                Description = response?[appId]["data"]["short_description"].ToString(),
                ImageUrl = response?[appId]["data"]["header_image"].ToString()
            };
            return game;
        }

        private async static Task<string[]> GetRandomAppIdsFromCategoryAsync(string args)
        {
            var url = $"https://steam.cma.dk/apps?non_vr=1&{args}";
            var response = await GetDataFromApiAsync(url);
            var appIds = response["data"].ToArray().Select(a => a["id"].ToString()).ToArray();
            return appIds;
        }

        private static async Task<JObject> GetDataFromApiAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                // Случай, когда ответ приходит в виде JSON массива
                if (json[0] == '[')
                {
                    json = "{\"data\":" + json + "}";
                }
                return JObject.Parse(json);
            }
            return null;
        }
    }

    class RecommendationData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
