using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrameBuffer
{
    public static Queue<List<Vector3>> buffer = new Queue<List<Vector3>>();
    public static List<Vector3> lastFrame = null;
    public static bool isActive = false;

    public static void Put(List<Vector3> list)
    {
        buffer.Enqueue(list);
    }

    public static void PutEmpty()
    {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < 17; i++)
            list.Add(new Vector3(0, 0, 0));
        buffer.Enqueue(list);
    }

    public static List<Vector3> Get()
    {
        while (buffer.Count > 0)
            lastFrame = buffer.Dequeue();
        return lastFrame;
    }
}
