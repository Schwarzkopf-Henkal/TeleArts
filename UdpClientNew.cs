using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;

public class UDPReceiver : MonoBehaviour
{
    public Image image;
    public Sprite sprite1;
    private Texture2D texture;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private byte[] receivedData;
    private bool dataReceived=false;
    private bool structHeadReceived=false;
    private int datasize;
    private int BUFSIZE = 65507;
    private List<byte> data_total = new List<byte>();
    private int recv_times = 0;
    void Start()
    {
        image.sprite = sprite1;
        texture = new Texture2D(640, 480);
        udpClient = new UdpClient(5005);  // ¼àÌý¶Ë¿ÚºÅ
        endPoint = new IPEndPoint(IPAddress.Any, 5005);
        udpClient.Client.ReceiveBufferSize = 131070;

        // Start receiving data
        Task.Run(() => ReceiveData());
    }

    async Task ReceiveData()
    {
        while (true)
        {
            try
            {
                var result = await udpClient.ReceiveAsync();
                receivedData = result.Buffer;
                //Debug.Log("Received data length: " + receivedData.Length);
                if (receivedData.Length == 4&&!structHeadReceived)
                {
                    structHeadReceived = true;
                    datasize= BitConverter.ToInt32(receivedData);
                    recv_times = (datasize - 1) / BUFSIZE + 1;
                    //Debug.Log("Recv Times" + recv_times);
                    Debug.Log("datasize" + datasize);
                }
                else if(structHeadReceived&&!dataReceived) {
                    data_total.AddRange(receivedData);
                    if (--recv_times==0)
                    {
                        //Debug.Log(data_total.ToArray().Length);
                        dataReceived = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }

    void Update()
    {
        if (dataReceived)
        {
            UpdateTexture();
            dataReceived = false;
            structHeadReceived = false;
            data_total.Clear();
        }
    }
    private bool flag = true;
    void UpdateTexture()
    {
        if (structHeadReceived&&dataReceived)
        {
            Debug.Log(data_total.ToArray().Length);
            if (flag)
            {
                using (var fileStream = File.Create("D:\\File.jpg"))
                {
                    fileStream.Write(data_total.ToArray());
                }
                flag = false;
            }
            texture.LoadImage(data_total.ToArray());
            texture.Apply();
            image.sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, 640, 480), new Vector2(0.5f, 0.5f));
        }
    }

    //void OnDestroy()
    //{
    //    udpClient.Close();
    //}
}
