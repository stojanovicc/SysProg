using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

class HttpServer
{
    static WebClient _client = new WebClient();
    public static WebClient client { get { return _client; } }

    public static async Task StartServerAsync()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Pokrenuli ste server.");
        Console.WriteLine("Server osluškuje na http://localhost:5000/");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            ThreadPool.QueueUserWorkItem(new WaitCallback(async (state) => await RequestAsync(context)), null);
        }
    }

    public static async Task RequestAsync(HttpListenerContext context)
    {
        string query = context.Request.RawUrl.Substring(1);

        if (string.IsNullOrEmpty(query))
        {
            await ResponseAsync(context, "Potrebno je da unesete query parametar.");
            return;
        }

        string responseString = await Museum.GetMuseumsAsync(query);
        await ResponseAsync(context, responseString);
    }

    public static async Task ResponseAsync(HttpListenerContext context, string responseString)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.ContentType = "text/html";
        Stream output = context.Response.OutputStream;
        await output.WriteAsync(buffer, 0, buffer.Length);
        output.Close();
    }
}
