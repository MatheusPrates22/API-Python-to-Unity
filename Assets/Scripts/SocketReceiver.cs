using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class SocketReceiver : MonoBehaviour
{
    public const int GATE = 12345;

    public int maxPacketSize = 1024;

    [SerializeField] private ScreenShotController _controller;
    [SerializeField] private Transform cylinderTransform;

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private bool needStartCoroutine;

    private void Start()
    {
        udpClient = new UdpClient(GATE);
        udpClient.BeginReceive(ReceiveCallback, null);
        // udpClient.BeginReceive(CoroutineCallback, null);
    }

    private void Update() {
        if (needStartCoroutine)
        {
            StartCoroutine(CaptureScreenshotAndSendResponse());
            needStartCoroutine = false;
        }
    }

    private void ReceiveCallback(System.IAsyncResult result)
    {
        //--------RECEIVE INFO---------
        endPoint = new IPEndPoint(IPAddress.Any, GATE);
        // IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, GATE);
        byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
        string receivedString = Encoding.ASCII.GetString(receivedBytes);
        Debug.Log("Mensagem recebida: " + receivedString);
        
        //---------TREAT INFO------------
        UnityAPIJsonFormat unityAPIJsonFormat = JsonUtility.FromJson<UnityAPIJsonFormat>(receivedString);
        _controller.Initialize(unityAPIJsonFormat);
        needStartCoroutine = true;

        // //---------RESPONSE------------
        // string response = "Resposta da Unity";
        // byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        // udpClient.Send(responseBytes, responseBytes.Length, endPoint);

        udpClient.BeginReceive(ReceiveCallback, null);
    }

    private IEnumerator CaptureScreenshotAndSendResponse()
    {
        //---------TIRA FOTO------------
        yield return new WaitForEndOfFrame();

        string screenshotPath = "Assets\\Photos\\screenshotCoroutine.png";
        ScreenCapture.CaptureScreenshot(screenshotPath);

        yield return new WaitForSeconds(0.2f);

        //---------CONVERTE EM BYTES------------
        byte[] imageBytes = File.ReadAllBytes(screenshotPath);

        //---------DIVIDE EM X PACOTES------------
        int numPackets = Mathf.CeilToInt((float)imageBytes.Length / maxPacketSize);
        Debug.Log("Number of packets: " + numPackets);

        //---------ENVIA O NÚMERO DE PACOTES------------
        string amountOfPackages = numPackets.ToString();
        byte[] responseBytes = Encoding.ASCII.GetBytes(amountOfPackages);
        udpClient.Send(responseBytes, responseBytes.Length, endPoint);

        yield return new WaitForSeconds(0.2f);

        //---------ENVIA OS PACOTES------------
        for (int i = 0; i < numPackets; i++)
        {
            int offset = i * maxPacketSize;
            int packetSize = Mathf.Min(maxPacketSize, imageBytes.Length - offset);
            byte[] packetData = new byte[packetSize];
            System.Array.Copy(imageBytes, offset, packetData, 0, packetSize);

            udpClient.Send(packetData, packetData.Length, endPoint);
        }
    }
}
