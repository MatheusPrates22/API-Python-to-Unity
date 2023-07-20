using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    public static TCPServer Instance { get; private set; }

    public static event EventHandler<string> OnReceiveCalled;

    [SerializeField] private int port = 12345;
    public int maxPacketSize = 1024;

    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer;
    private string receivedMessage = "";

    void Start()
    {
        // Inicia o servidor TCP
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        server.Start();
        Debug.Log("Servidor TCP iniciado. Aguardando conexão local...");
        
        // Aguarda a conexão do cliente
        server.BeginAcceptTcpClient(AcceptClientCallback, null);
    }

    private void AcceptClientCallback(System.IAsyncResult asyncResult)
    {
        try
        {
            // Cliente conectado
            client = server.EndAcceptTcpClient(asyncResult);
            stream = client.GetStream();
            Debug.Log("Cliente local conectado.");

            // Inicia a leitura assíncrona das mensagens do cliente
            buffer = new byte[maxPacketSize];
            stream.BeginRead(buffer, 0, buffer.Length, ReceiveCallback, null);
            

            // Inicia a espera por novas conexões
            server.BeginAcceptTcpClient(AcceptClientCallback, null);
        }
        catch (Exception e)
        {
            // Ocorreu um erro ao aceitar o cliente
            Debug.LogError("Erro ao aceitar cliente: " + e.Message);
        }
    }

    private void ReceiveCallback(System.IAsyncResult ar)
    {
        // Lê a mensagem recebida do cliente
        int bytesRead = stream.EndRead(ar);
        // buffer = (byte[])ar.AsyncState;
        buffer = new byte[client.ReceiveBufferSize];
        receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        // Processa a mensagem recebida
        Debug.Log("Receive Callback: " + receivedMessage);
        // APIController.MessageReceived(receivedMessage);
        // OnReceiveCalled?.Invoke(this, receivedMessage);

        // Reinicia a leitura assíncrona das mensagens
        stream.BeginRead(buffer, 0, buffer.Length, ReceiveCallback, null);
    }

    #region SEND MESSAGE
    public static void SendMessageCallback(Texture2D texture, bool sendNumPackets = true ){
        byte[] messageBytes = texture.EncodeToPNG();
        SendMessageCallback(messageBytes, sendNumPackets);
    }

    public static void SendMessageCallback(string message, bool sendNumPackets = true) {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        SendMessageCallback(messageBytes, sendNumPackets);
    }

    public static void SendMessageCallback(byte[] bytesToSend, bool sendNumPackets = true) {
        try {
            Instance.StartCoroutine(SendMessageCoroutine(bytesToSend, sendNumPackets));
        } catch (Exception e) {
            Debug.Log(e.ToString());
        }
    }

    private static IEnumerator SendMessageCoroutine(byte[] bytesToSend, bool sendNumPackets) {
        int numPackets = Mathf.CeilToInt((float)bytesToSend.Length / Instance.maxPacketSize);

        //---------MANDA NUMERO DE PACOTES SE NECESSARIO------------
        if (sendNumPackets)
        {
            byte[] numPacketsBytes = Encoding.ASCII.GetBytes(numPackets.ToString());
            Instance.stream.Write(numPacketsBytes, 0, numPacketsBytes.Length);
        }

        for (int i = 0; i < numPackets; i++) {
            int offset = i * Instance.maxPacketSize;
            int packetSize = Mathf.Min(Instance.maxPacketSize, bytesToSend.Length - offset);
            byte[] packet = new byte[packetSize];
            System.Array.Copy(bytesToSend, offset, packet, 0, packetSize);

            Instance.stream.Write(packet, 0, packet.Length);
            yield return new WaitForSeconds(0.001f);
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        // Fecha a conexão quando a aplicação é encerrada
        if (stream != null)
        {
            stream.Close();
        }

        if (client != null)
        {
            client.Close();
        }

        if (server != null)
        {
            server.Stop();
        }
    }
}
