using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static readonly WebClient client = new WebClient();
    static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

    public class Artwork
    {
        public string title { get; set; }
        public string api_link { get; set; }
    }

    public class ArtworkResponse
    {
        public List<Artwork> data { get; set; }
    }

    static void Main()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Server je pokrenut");
        Console.WriteLine("http://localhost:5000/");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessRequest), context);
        }
    }

    static void ProcessRequest(object state)
    {
        HttpListenerContext context = (HttpListenerContext)state;
        string query = context.Request.RawUrl.Substring(1);
        if (string.IsNullOrEmpty(query))
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("<html><body>Unesi parametar.</body></html>");
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = "text/html";
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            return;
        }
        string responseString = GetArtworks(query);

        byte[] buffer2 = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer2.Length;
        context.Response.ContentType = "text/html";
        Stream output2 = context.Response.OutputStream;
        output2.Write(buffer2, 0, buffer2.Length);
        output2.Close();
    }

    static string GetArtworks(string query)
    {
        query = query.Replace('_', ' ');

        if (cache.ContainsKey(query))
        {
            return cache[query];
        }

        try
        {
            string url = $"https://api.artic.edu/api/v1/artworks/search?q={query}&limit=100";
            string responseBody = client.DownloadString(url);
            var artworkResponse = JsonConvert.DeserializeObject<ArtworkResponse>(responseBody);
            if (artworkResponse.data.Count == 0)
            {
                return "<html><body>Greska: Nema umetnickih dela koja zadovoljavaju vasu pretragu.</body></html>";
            }
            string result = "<html><body>";
            foreach (var artwork in artworkResponse.data)
            {
                result += $"<p>{artwork.title}</p>";
            }
            result += "</body></html>";

            cache.Add(query, result);

            return result;
        }
        catch (Exception)
        {
            return "<html><body>Greska: Umetnicka dela nisu pronadjena.</body></html>";
        }
    }
}
