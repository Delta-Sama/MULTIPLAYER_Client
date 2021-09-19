using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
            SendMessageToHost("Hello from client");

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            // Receive a message and put it into a byte recBuffer
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            // Process received event depending on its type
            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    // Decrypt the message from bytes to string
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            // Initialize NetworkTransport before its usage
            NetworkTransport.Init();

            // Create a config which describes channel types, various timeouts and sizes
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable); // Guarantee of delivering but not ordering
            unreliableChannelID = config.AddChannel(QosType.Unreliable); // No guarantee of delivering or ordering

            // Define the number of default connections and number of special connections
            HostTopology topology = new HostTopology(config, maxConnections);
            // Add a host based on our topology and bind the socket to a random port (since 0 as a port is provided)
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            // Connect to a peer with a chosen address and socket
            connectionID = NetworkTransport.Connect(hostID, "192.168.0.11", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        // Send a peer a message of disconnection and disconnect
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        // Convert a string to a byte buffer
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        // Send a message(byte buffer) to the peer
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}
