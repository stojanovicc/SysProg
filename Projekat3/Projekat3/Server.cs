using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class Server
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly ArticleService service = new ArticleService();

        public Server()
        {
            listener.Prefixes.Add("http://localhost:5050/");
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Server listening on port 5050...");
            while (true)
            {
                var context = await listener.GetContextAsync();
                Task.Run(() => HandleRequestAsync(context));
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            if (context.Request.Url?.AbsolutePath == "/favicon.ico")
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
                return;
            }

            Console.WriteLine("Request: " + context.Request.HttpMethod + " " + context.Request.Url);
            var keyword = context.Request.QueryString["keyword"];
            var category = context.Request.QueryString["category"];


            if (string.IsNullOrEmpty(keyword))
            {
                Console.WriteLine("Progesan zahtev! Keyword parametar nedostaje!");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Keyword parametar nedostaje! Zahtev treba da izgleda kao: 'http://localhost:5050/?keyword=KEY_WORD&category=VALID_CATEGORY'");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;
            }
            string[] categories = { "business", "entertainment", "general", "health", "science", "sports", "technology" };
            bool goodCateg = false;
            if (string.IsNullOrEmpty(category))
            {
                Console.WriteLine("Pogresan zahtev! Category parametar nedostaje!");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Category parametar nedostaje! Zahtev treba da izgleda kao: 'http://localhost:5050/?keyword=KEY_WORD&category=VALID_CATEGORY'");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;

            }
            foreach (string categ in categories)
            {
                if (categ == category)
                {
                    goodCateg = true;
                }
            }
            if (!goodCateg)
            {
                Console.WriteLine("Pogresan zahtev! Category parametar nije validan!");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorBytes = Encoding.UTF8.GetBytes("Category parametar nije prepoznat. Validne kategorije su: \"business\", \"entertainment\", \"general\", \"health\", \"science\", \"sports\", \"technology\"");
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;
            }

            var observer = new ArticleObserver("ArticleObserver");
            var subject = new ArticleStream();


            var articles = await service.FetchArticles(keyword, category);
            subject.Subscribe(observer);

            await subject.GetArticlesAsync(keyword, category);
            if (!subject.HasArticles)
            {
                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes("There are no articles with such keyword!");
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                var topics = TopicModelling.GetTopics(articles.Select(a => a.Title));
                var json = new JObject
                {
                    ["articles"] = new JArray(articles.Select(a => new JObject
                    {
                        ["title"] = a.Title,
                        ["source"] = a.Source,
                        ["topics"] = new JArray(a.Topics.Select(t => new JValue(t))),
                    })),
                    ["all articles topics:"] = new JArray(topics)
                };
                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                Console.WriteLine("Zahtev je uspesno obradjen i odgovor je poslat klijentu!");
            }

        }
    }
}
