using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;

public class UDPServer : MonoBehaviour
{
    public static UDPServer Instance { get; private set; }

    public static event EventHandler<string> OnReceiveCalled;

    [Header("Socket")]
    public int port = 12345;
    public int maxPacketSize = 1024;

    private UdpClient udpClient;
    private IPEndPoint endPoint;

    private bool needStartCoroutine;

    // private Vector2 photoResolution;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    private void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(System.IAsyncResult result)
    {
        //--------RECEIVE INFO---------
        endPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
        string receivedString = Encoding.ASCII.GetString(receivedBytes);
        Debug.Log("Mensagem recebida: " + receivedString);
        
        //---------TREAT INFO------------
        OnReceiveCalled?.Invoke(this, receivedString);

        //Volta a ouvir nova chamada
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    #region SEND MESSAGE
    public static void SendMessageCallback(Texture2D texture, bool sendNumPackets = true)
    {
        byte[] messageBytes = texture.EncodeToPNG();
        SendMessageCallback(messageBytes, sendNumPackets);
    }

    public static void SendMessageCallback(string message, bool sendNumPackets = true)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        SendMessageCallback(messageBytes, sendNumPackets);
    }

    public static void SendMessageCallback(byte[] messageBytes, bool sendNumPackets = true)
    {
        try
        {
            Instance.StartCoroutine(SendMessageCoroutine(messageBytes, sendNumPackets));
        } catch (Exception e){
            Debug.Log(e.ToString());
        }

    }

    private static IEnumerator SendMessageCoroutine(byte[] bytesToSend, bool sendNumPackets)
    {
        int numPackets = Mathf.CeilToInt((float)bytesToSend.Length / Instance.maxPacketSize);
        //---------DIVIDE EM X PACOTES------------
        if (sendNumPackets)
        {
            byte[] numPacketsBytes = Encoding.ASCII.GetBytes(numPackets.ToString());
            Instance.udpClient.Send(numPacketsBytes, numPacketsBytes.Length, Instance.endPoint);
        }

        for (int i = 0; i < numPackets; i++)
        {
            int offset = i * Instance.maxPacketSize;
            int packetSize = Mathf.Min(Instance.maxPacketSize, bytesToSend.Length - offset);
            byte[] packetData = new byte[packetSize];
            System.Array.Copy(bytesToSend, offset, packetData, 0, packetSize);

            Instance.udpClient.Send(packetData, packetData.Length, Instance.endPoint);
            yield return new WaitForSeconds(0.001f);
        }
    }
    #endregion

    private void OnApplicationQuit() {
        udpClient.Close();
    }
}
