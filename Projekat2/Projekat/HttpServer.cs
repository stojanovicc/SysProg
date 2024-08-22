using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

class HttpServer
{
    public static HttpClient client = new HttpClient();
    public static async Task StartServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Server je pokrenut");
        Console.WriteLine("http://localhost:5000/");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            await Task.Run(() => Program.ProcessRequest(context));
        }
    }
}