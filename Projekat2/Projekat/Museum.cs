using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Museum
{
    public string title { get; set; }

    public Museum()
    {
        title = " ";
    }

    public static async Task<string> GetMuseums(string query)
    {
        query = query.Replace('_', ' ');

        if (MuseumCache.TryGetFromCache(query, out var cachedResult))
        {
            return cachedResult;
        }

        try
        {
            string searchUrl = $"https://collectionapi.metmuseum.org/public/collection/v1/search?q={query}";
            HttpServer.client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string searchResponseBody = await HttpServer.client.GetStringAsync(searchUrl);

            var searchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(searchResponseBody);

            if (!searchResponse.TryGetValue("objectIDs", out var objectIdsToken) || objectIdsToken == null)
            {
                return "<html><body>Greska: Nema umetnickih dela koja zadovoljavaju vasu pretragu.</body></html>";
            }

            var objectIDs = JsonConvert.DeserializeObject<List<int>>(objectIdsToken.ToString());

            if (objectIDs == null || objectIDs.Count == 0)
            {
                return "<html><body>Greska: Nema umetnickih dela koja zadovoljavaju vasu pretragu.</body></html>";
            }

            string result = "<html><body>";
            int count = 0;

            foreach (var objectId in objectIDs)
            {
                if (count >= 5) break;

                string artworkUrl = $"https://collectionapi.metmuseum.org/public/collection/v1/objects/{objectId}";
                string artworkResponseBody = await HttpServer.client.GetStringAsync(artworkUrl);
                var artwork = JsonConvert.DeserializeObject<Dictionary<string, object>>(artworkResponseBody);

                if (artwork.TryGetValue("title", out var title))
                {
                    result += $"<p>{title}</p>";
                }

                count++;
            }

            result += "</body></html>";

            MuseumCache.AddToCache(query, result);

            return result;
        }
        catch (Exception e)
        {
           // Console.WriteLine(e);
            return "<html><body>Ne moze da se pribavi trazeni title.</body></html>";
        }
    }
}

