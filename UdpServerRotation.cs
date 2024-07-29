using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using Newtonsoft.Json;
using OpenCvSharp;
using System.Text;
public class UdpServerRotation : MonoBehaviour
{
    public Image image;
    public bool IsUdpcServStart = false;
    private UdpClient udpServ;
    // Start is called before the first frame update
    void Start()
    {
        udpServ = new UdpClient();
        //udpServ.Connect("127.0.0.1", 5006);
        IsUdpcServStart = true;
        //SendRotation();
        InvokeRepeating("SendRotation", 0f, 0.5f);
    }
    // Update is called once per frame
    void SendRotation()
    {
        //while (IsUdpcServStart)
        //{
        IPEndPoint remotePoint = new IPEndPoint(IPAddress.Parse("192.168.137.1"),5006);
            try
            {
                Vector3 eularAngles = transform.rotation.eulerAngles;
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new float[] { eularAngles.x, eularAngles.y, eularAngles.z }));
                udpServ.Send(data, data.Length,remotePoint);
                Debug.Log("Sent data length:" + data.Length);
            }
            catch (Exception ex)
            {
                Debug.Log("SendErrorMessage:" + ex.Message);
            }
        //}
    }
    void Update()
    {
        
    }
}
