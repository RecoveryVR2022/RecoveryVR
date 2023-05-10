using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HttpReciver : MonoBehaviour
{
    private HttpListener listener;

    void Start()
    {
        Task.Run(async () =>
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:3000/");
            listener.Start();
            Debug.Log("Server started on port 3000");

            while (true)
            {
                Debug.Log("Waiting for client connection...");
                HttpListenerContext context = await listener.GetContextAsync();
                Debug.Log("Client connected");

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Enable CORS
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string content = await reader.ReadToEndAsync();
                        Debug.Log("Received message: " + content);
                    }
                }

                string responseString = "OK";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
            }
        });
    }

    void OnDestroy()
    {
        if (listener != null)
        {
            listener.Stop();
        }
    }
}
