using Newtonsoft.Json;
using Projekat;
using System.Net;

public class Museum
{
    public string name { get; set; }
    public string apiLink { get; set; }

    private static readonly object cacheLock = new object();

    public static string GetMuseums(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Doslo je do greske: Unesite validan query parametar.";
            }

            lock (cacheLock)
            {
                if (MuseumCache.cache.ContainsKey(query))
                {
                    return MuseumCache.cache[query];
                }
            }

            string url = $"https://collectionapi.metmuseum.org/public/collection/v1/search?q={query}";
            string response = new WebClient().DownloadString(url);
            dynamic museumResponse = JsonConvert.DeserializeObject(response);

            if (museumResponse.total == 0)
            {
                return "Nema rezultata pretrage za dati upit.";
            }

            var objectIDs = new List<int>();
            foreach (var objectId in museumResponse.objectIDs)
            {
                objectIDs.Add((int)objectId);
            }

            var resultObject = new
            {
                total = museumResponse.total,
                objectIDs = objectIDs
            };

            string resultJson = JsonConvert.SerializeObject(resultObject, Formatting.Indented);

            lock (cacheLock)
            {
                MuseumCache.cache.Add(query, resultJson);
            }

            return resultJson;
        }
        catch (WebException webEx)
        {
            if (webEx.Response is HttpWebResponse httpResponse)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return "Doslo je do greske: Stranica nije pronadjena (404).";
                }
                else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    return "Doslo je do greske: Neispravan zahtev (400).";
                }
                else
                {
                    return "Doslo je do greške prilikom obrade HTTP zahteva.";
                }
            }
            else
            {
                return "Doslo je do greške prilikom komunikacije sa serverom.";
            }
        }
        catch (Exception)
        {
            return "Doslo je do nepoznate greške prilikom obrade zahteva.";
        }
    }
}


