using WebSocketSharp;
using UnityEngine;
using System.IO;

public class WS_Client : MonoBehaviour
{
    public static WS_Client Instance;
    private WebSocket ws;
    void Awake()
    {
        Instance = this;
    }
    public void Connect(string ip, int ID)
    {
        ws = new WebSocket($"ws://{ip}:8080/");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Подключено к серверу");
            if (ID == 0)
                UIManager.Instance.ShowStrings();
            else
                UIManager.Instance.ShowBow();
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Отключено от сервера");
        };

        ws.Connect();
    }

    public void Send(string msg)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Send(msg);
        }
    }

    public void SendBinary(int stringNumber, float pitch)
    {
        Debug.Log("Отправлено сообщение");

        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            //writer.Write(myId);
            writer.Write(stringNumber);
            writer.Write(pitch);

            ws.Send(ms.ToArray());
        }
    }
}