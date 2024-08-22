using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

class Program
{
    static void Main()
    {
        MuseumCache.cacheCleanupTimer.Elapsed += (sender, e) => MuseumCache.CacheCleanup();
        HttpServer.StartServer().GetAwaiter().GetResult();
    }

    public static async Task ProcessRequest(object state)
    {
        HttpListenerContext context = (HttpListenerContext)state;
        string query = context.Request.RawUrl.Substring(1);
        if (string.IsNullOrEmpty(query))
        {
            byte[] buffer = Encoding.UTF8.GetBytes("<html><body>Unesi parametar.</body></html>");
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = "text/html";
            Stream output = context.Response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
            return;
        }
        string responseString = await Museum.GetMuseums(query);

        byte[] buffer2 = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer2.Length;
        context.Response.ContentType = "text/html";
        Stream output2 = context.Response.OutputStream;
        await output2.WriteAsync(buffer2, 0, buffer2.Length);
        output2.Close();
    }
}