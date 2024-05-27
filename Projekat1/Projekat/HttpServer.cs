using System;
using System.Net;
using System.Threading;

class HttpServer
{
    static WebClient _client = new WebClient();
    public static WebClient client { get { return _client; } }

    public static void StartServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Pokrenuli ste server.");
        Console.WriteLine("Server osluškuje na http://localhost:5000/");


        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Program.Request), context);
        }
    }

}
