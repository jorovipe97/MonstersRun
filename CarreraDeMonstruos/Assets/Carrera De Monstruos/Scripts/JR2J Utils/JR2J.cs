using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets; // Needed for the TcpListener
using UnityEngine.Events;



public class JR2J : MonoBehaviour {
    [System.Serializable]
    public struct JR2JMessage
    {
        public string address;
        public int value;
    }

    // JR2JMessage event for unity receiving the address.
    [System.Serializable]
    public class IncommingMessage : UnityEvent<JR2JMessage>
    {

    }

    public string IP = "192.168.1.21";
    public int PORT = 80;
    private IPAddress localhost;

    // Message listener
    public IncommingMessage onIncommingMessage;

    // Docs https://msdn.microsoft.com/es-es/library/system.net.sockets.tcplistener(v=vs.110).aspx
    TcpListener tcpServer;

    // Buffer for reading data
    Byte[] bytes = new Byte[256];
    String data = null;

    

    // List of all available tcp clients
    public List<TcpClient> tcpClients;
    // Client streams
    public List<NetworkStream> streams;

    // Use this for initialization
    void Start () {
        localhost = IPAddress.Parse(IP);
        tcpServer = new TcpListener(localhost, PORT);

        // Starts server
        tcpServer.Start();

        // Initializes the list of tcp clients
        tcpClients = new List<TcpClient>();
        // Initializes the list of client streams
        streams = new List<NetworkStream>();

        Application.runInBackground = true;
    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            if ()
            // If there are clients in thw connection queue
            if (tcpServer.Pending())
            {
                Debug.Log("Pending client");
                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = tcpServer.AcceptTcpClient();
                // Blocks until the client gets connected
                // while (!client.Connected) { }
                Debug.Log("Client connected");
                tcpClients.Add(client);
                NetworkStream clientStream = client.GetStream();
                // Blocks until stream can be readed
                // while (!clientStream.CanRead) { }
                streams.Add(clientStream);
            }

            // Make sure there are connected client's stream
            if (streams.Count == 0)
            {
                // Debug.Log("Returning for count");
                return;
            }
            // Debug.Log("Stream Available");
            // Reads the data from each network stream
            foreach (NetworkStream stream in streams)
            {
                // TCP server frezes unity https://stackoverflow.com/questions/19046251/reading-tcp-data-freezes-unity
                StringBuilder str = new StringBuilder();
                int numberOfBytesReaded = 0;
                // Incoming message may be larger than the buffer size.
                do
                {
                    Debug.Log("Reading str");
                    numberOfBytesReaded = stream.Read(bytes, 0, bytes.Length);
                    str.AppendFormat("{0}", System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesReaded));
                }
                while (stream.DataAvailable);

                Debug.Log("Inifine loop");
                JR2JMessage msg = new JR2JMessage();
                // If the separator parameter is null or contains no characters, white-space characters are assumed to be the delimiters.

                string[] parameters = str.ToString().Split(' ');
                if (parameters.Length == 1)
                    return;
                msg.address = parameters[0];
                
                bool converted = Int32.TryParse(parameters[1], out msg.value);

                if (converted)
                    onIncommingMessage.Invoke(msg);
                else
                    Debug.Log("Could not do the conversion :/");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            CloseClientsConnections();
        }
    }

    [ContextMenu("Close all client connections")]
    public void CloseClientsConnections()
    {
        for(int i = 0; i < tcpClients.Count; i++)
        {
            CloseClient(i);
        }        
    }

    public void CloseClient(int i)
    {
        tcpClients[i].Close();
        tcpClients.RemoveAt(i);
        streams[i].Close();
        streams.RemoveAt(i);
    }
}
