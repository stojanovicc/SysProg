using Newtonsoft.Json;
using Projekat;
using System.Net;
using System.Text.RegularExpressions;

public class Museum
{
    public string name { get; set; }
    public string apiLink { get; set; }

    public static string GetMuseums(string query)
    {
        try
        {
            if (MuseumCache.cache.ContainsKey(query))
            {
                return MuseumCache.cache[query];
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return "<html><body>Doslo je do greske: Unesite validan query parametar.</body></html>";
            }


            string url = $"https://collectionapi.metmuseum.org/public/collection/v1/search?q={query}";
            string response = new WebClient().DownloadString(url);
            dynamic museumResponse = JsonConvert.DeserializeObject(response);

            if (museumResponse.total == 0)
            {
                return "<html><body> Nema rezultata pretrage za dati upit. </body></html>";
            }

            string result = "<html><body>";

            foreach (var objectId in museumResponse.objectIDs)
            {
                result += $"<p>{objectId}</p>";
            }

            result += "</body></html>";

            MuseumCache.cache.Add(query, result);

            return result;
        }
        catch (WebException webEx)
        {
            if (webEx.Response is HttpWebResponse httpResponse)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return "<html><body>Doslo je do greške: Stranica nije pronađena (404).</body></html>";
                }
                else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    return "<html><body>Doslo je do greške: Neispravan zahtev (400).</body></html>";
                }
                else
                {
                    return "<html><body>Doslo je do greške prilikom obrade HTTP zahteva.</body></html>";
                }
            }
            else
            {
                return "<html><body>Doslo je do greške prilikom komunikacije sa serverom.</body></html>";
            }
        }
        catch (Exception)
        {
            return "<html><body>Doslo je do nepoznate greške prilikom obrade zahteva.</body></html>";
        }
    }
}
