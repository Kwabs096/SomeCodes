using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using System.IO;

public class WS_Server : MonoBehaviour
{
    private WebSocketServer wssv;

    void Start()
    {
        wssv = new WebSocketServer(8080);
        wssv.AddWebSocketService<GameService>("/");
        wssv.Start();

        Debug.Log("Server launched");
    }

    void OnApplicationQuit()
    {
        wssv.Stop();
    }
}

public class GameService : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        if (!e.IsBinary)
        {
            Debug.LogWarning("Data is not binary");
            return;
        }
        //Debug.Log("Получено сообщение");

        using (MemoryStream ms = new MemoryStream(e.RawData))
        using (BinaryReader reader = new BinaryReader(ms))
        {
            //int playerId = reader.ReadInt32();
            int intValue = reader.ReadInt32();
            float floatValue = reader.ReadSingle();
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                GameManager.Instance.OnDataReceived(intValue, floatValue);
            });
            Debug.Log($"int: {intValue} | float: {floatValue}");
        }
    }

    protected override void OnOpen()
    {
        Debug.Log("Client connected");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log("Client disconnected");
    }
}