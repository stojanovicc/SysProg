using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public static class NewsApiClient
    {
        public static IObservable<List<Article>> GetNewsArticlesAsync(string category, string keyword)
        {
            return Observable.FromAsync(async () =>
            {
                string apiKey = "2ac42a722140420989888a3ff2ef5550";
                string requestUri = $"https://newsapi.org/v2/top-headlines?category={category}&q={keyword}&pageSize=100&apiKey={apiKey}";

                Console.WriteLine("Request URI: " + requestUri);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("ConsoleApp/1.0");

                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    Console.WriteLine("Response status code: " + response.StatusCode);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Error body: " + errorBody);
                    }

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(responseBody);
                    var articles = json["articles"]
                        .Select(a => new Article
                        {
                            Title = (string)a["title"],
                            Source = GetSourceName(a),
                            Description = (string)a["description"]
                        }).ToList();

                    return articles;
                }
            });
        }

        private static string GetSourceName(JToken article)
        {
            if (article["source"] != null && article["source"]["name"] != null)
            {
                return (string)article["source"]["name"];
            }
            else
            {
                return "Unknown";
            }
        }

        public static List<string> ExtractTopics(IEnumerable<string> titles)
        {
            var topics = new List<string>();

            foreach (var title in titles)
            {
                var words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    topics.Add(word.ToLower());
                }
            }

            return topics.Distinct().ToList();
        }
    }
}