using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPThread : MonoBehaviour
{
	private static TCPThread Instance;

    private static TcpListener server;
    private static TcpClient client;
    private Thread clientReceiveThread;

	private void Awake() {
		if (Instance == null) Instance = this; else Destroy(Instance);
	}

    private void Start()
    {
        StartServer();
    }

	public static void SendTexture (Texture2D texture) {
		SendMessageCallback(texture);
	}

    private void StartServer() {
        clientReceiveThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        clientReceiveThread.IsBackground = true;
        clientReceiveThread.Start();
    }

    private void ListenForIncommingRequests() {
        try {
            server = new TcpListener(IPAddress.Parse(APIController.host), APIController.port); 			
			server.Start();              
			Debug.Log("Server is listening");              
			Byte[] buffer = new Byte[APIController.bufferSize];  			
			while (true) { 				
				using (client = server.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = client.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(buffer, 0, buffer.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(buffer, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							
							// Debug.Log("client message received as: " + clientMessage);
                            APIController.ReadAPIJsonFile(clientMessage); 						
						} 					
					} 				
				} 			
			} 		
        } catch (SocketException socketException) {
            Debug.Log("Socket Exception " + socketException);
        }
    }

    private void SendMessage() { 		
		if (client == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = client.GetStream(); 			
			if (stream.CanWrite) {                 
				string serverMessage = "This is a message from your server."; 			
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
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
		if (client != null) {             
			NetworkStream stream = client.GetStream();
			if (stream.CanWrite) {
				int numPackets = Mathf.CeilToInt((float)bytesToSend.Length / APIController.bufferSize);

				//---------MANDA NUMERO DE PACOTES SE NECESSARIO------------
				if (sendNumPackets)
				{
					byte[] numPacketsBytes = Encoding.ASCII.GetBytes(numPackets.ToString());
					stream.Write(numPacketsBytes, 0, numPacketsBytes.Length);
				}

				//---------MANDA CADA PARTE DO PACOTE------------
				for (int i = 0; i < numPackets; i++) {
					int offset = i * APIController.bufferSize;
					int packetSize = Mathf.Min(APIController.bufferSize, bytesToSend.Length - offset);
					byte[] packet = new byte[packetSize];
					System.Array.Copy(bytesToSend, offset, packet, 0, packetSize);

					stream.Write(packet, 0, packet.Length);
					yield return new WaitForSeconds(0.001f);
				}
			}
		}  		
    }
    #endregion

}
