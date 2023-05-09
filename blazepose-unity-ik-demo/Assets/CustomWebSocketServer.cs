using Newtonsoft.Json;
using System.Collections.Generic;
using WebSocketServer;
using UnityEngine;

public class CustomWebSocketServer : WebSocketServer.WebSocketServer
{
    int frame_count = 0;

    override public void OnOpen(WebSocketConnection connection)
    {
        // Here, (string)connection.id gives you a unique ID to identify the client.
        Debug.Log(connection.id);
    }

    override public void OnMessage(WebSocketMessage message)
    {
        // (WebSocketConnection)message.connection gives you the connection that send the message.
        // (string)message.id gives you a unique ID for the message.
        // (string)message.data gives you the message content.
        List<Vector3> list = JsonConvert.DeserializeObject<List<Vector3>>(message.data);
        FrameBuffer.Put(list);
        FrameBuffer.isActive = true;
        //Debug.Log("web frame: " + frame_count);
        frame_count++;
    }

    override public void OnClose(WebSocketConnection connection)
    {
        // Here is the same as OnOpen
        Debug.Log(connection.id);
    }

}