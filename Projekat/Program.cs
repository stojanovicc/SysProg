using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Pokrenut je server.");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Request), context);
        }
    }
    public static void Request(object state)
    {
        HttpListenerContext context = (HttpListenerContext)state;
        string query = context.Request.RawUrl.Substring(1);

        if (string.IsNullOrEmpty(query))
        {
            Response(context, "<html><body>Potrebno je da unesete query parametar.</body></html>");
            return;
        }

        string responseString = Museum.GetMuseums(query);
        Response(context, responseString);
    }

    public static void Response(HttpListenerContext context, string responseString)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.ContentType = "text/html";
        Stream output = context.Response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }
}
