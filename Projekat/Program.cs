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
        HttpServer.StartServer();
    }
    public static void Request(object state)
    {
        HttpListenerContext context = (HttpListenerContext)state;
        string query = context.Request.RawUrl.Substring(1);

        if (string.IsNullOrEmpty(query))
        {
            Response(context, "Potrebno je da unesete query parametar.");
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
