using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Projekat;

public class Museum
{
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public string name { get; set; }
    public string apiLink { get; set; }

    public static async Task<string> GetMuseumsAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Došlo je do greške: Unesite validan query parametar.";
            }

            await semaphore.WaitAsync();
            try
            {
                string url = $"https://collectionapi.metmuseum.org/public/collection/v1/search?q={query}";
                string response = await HttpServer.client.DownloadStringTaskAsync(url);
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

                MuseumCache.Add(query, resultJson);

                return resultJson;
            }
            finally
            {
                semaphore.Release();
            }
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
                    return "Doslo je do greske prilikom obrade HTTP zahteva.";
                }
            }
            else
            {
                return "Doslo je do greske prilikom komunikacije sa serverom.";
            }
        }
        catch (Exception)
        {
            return "Doslo je do nepoznate greske prilikom obrade zahteva.";
        }
    }
}


//VRACANJE NASLOVA UMETNICKIH DELA

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;

//public class Museum
//{
//    public string name { get; set; }
//    public string apiLink { get; set; }

//    public static async Task<string> GetMuseumsAsync(string query)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(query))
//            {
//                return "Doslo je do greske: Unesite validan query parametar.";
//            }

//            string url = $"https://collectionapi.metmuseum.org/public/collection/v1/search?q={query}";
//            string response = await new WebClient().DownloadStringTaskAsync(url);
//            dynamic museumResponse = JObject.Parse(response);

//            if (museumResponse.total == 0)
//            {
//                return "Nema rezultata pretrage za dati upit.";
//            }

//            var objectIDs = museumResponse.objectIDs;
//            if (objectIDs == null || objectIDs.Count == 0)
//            {
//                return "Doslo je do greske: Nema informacija o umetničkim delima.";
//            }

//            var artworkTitles = new List<string>();
//            foreach (var objectId in objectIDs)
//            {
//                string title = await GetArtworkTitleAsync((int)objectId);
//                artworkTitles.Add(title);
//            }

//            // Formiranje rezultata
//            var resultObject = new
//            {
//                total = museumResponse.total,
//                artworkTitles = artworkTitles
//            };

//            string resultJson = JsonConvert.SerializeObject(resultObject, Formatting.Indented);

//            return resultJson;
//        }
//        catch (WebException webEx)
//        {
//            if (webEx.Response is HttpWebResponse httpResponse)
//            {
//                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
//                {
//                    return "Doslo je do greske: Stranica nije pronadjena (404).";
//                }
//                else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
//                {
//                    return "Doslo je do greske: Neispravan zahtev (400).";
//                }
//                else
//                {
//                    return $"Doslo je do greške prilikom obrade HTTP zahteva. Status koda: {httpResponse.StatusCode}";
//                }
//            }
//            else
//            {
//                return "Doslo je do greške prilikom komunikacije sa serverom.";
//            }
//        }
//        catch (Exception ex)
//        {
//            return $"Doslo je do nepoznate greške prilikom obrade zahteva. Greška: {ex.Message}";
//        }
//    }

//    private static async Task<string> GetArtworkTitleAsync(int objectId)
//    {
//        try
//        {
//            string url = $"https://collectionapi.metmuseum.org/public/collection/v1/objects/{objectId}";
//            string response = await new WebClient().DownloadStringTaskAsync(url);
//            dynamic artworkResponse = JObject.Parse(response);
//            return artworkResponse["title"].ToString();
//        }
//        catch (WebException webEx)
//        {
//            if (webEx.Response is HttpWebResponse httpResponse)
//            {
//                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
//                {
//                    return $"Doslo je do greske: Umetničko delo sa ID-jem {objectId} nije pronađeno (404).";
//                }
//                else
//                {
//                    return $"Doslo je do greške prilikom obrade HTTP zahteva za umetničko delo sa ID-jem {objectId}. Status koda: {httpResponse.StatusCode}";
//                }
//            }
//            else
//            {
//                return $"Doslo je do greške prilikom komunikacije sa serverom prilikom dohvatanja umetničkog dela sa ID-jem {objectId}.";
//            }
//        }
//        catch (Exception ex)
//        {
//            return $"Doslo je do nepoznate greške prilikom obrade zahteva za umetničko delo sa ID-jem {objectId}. Greška: {ex.Message}";
//        }
//    }

//}
