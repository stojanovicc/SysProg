using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    internal class ArticleService
    {
        private readonly string api_key = "2ac42a722140420989888a3ff2ef5550";

        public async Task<IEnumerable<Article>> FetchArticles(string keyword, string category)
        {
            HttpClient client = new HttpClient();
            var url = $"https://newsapi.org/v2/top-headlines?category={category}&q={keyword}&apiKey={api_key}";


            client.DefaultRequestHeaders.Add("User-Agent", "Projekat3/1.0");

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API response error: {errorContent}");
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);
                var articlesJson = jsonResponse["articles"];

                if (articlesJson == null)
                {
                    return Enumerable.Empty<Article>();
                }
                if(articlesJson.Count() == 0)
                {
                    Console.WriteLine("Ne postoje clanak sa ovom kljucnom recju!");
                    return Enumerable.Empty<Article>();
                }
                var articles = articlesJson.Select(article => new Article
                {

                    Title = (string)article["title"],
                    Source = (JObject)article["source"],
                    Topics = TopicModelling.GetTopics(new List<string> { (string)article["title"] })

                });
                return articles;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"API HTTP Request Error: {httpEx.Message}");
                return Enumerable.Empty<Article>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal API Error: {ex.Message}");
                return Enumerable.Empty<Article>();
            }
        }
    }
}
