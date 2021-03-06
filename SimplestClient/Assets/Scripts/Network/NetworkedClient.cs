using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{
    public string login;
    public string password;

    public static NetworkedClient Instance;

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
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
    
    public void SendServerRequest(string msg)
    {
        // Convert a string to a byte buffer
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        // Send a message(byte buffer) to the peer
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("[SERVER]: " + msg);

        string[] csv = msg.Split(',');

        int requestType = int.Parse(csv[0]);

        if (requestType == ServerToClientTransferSignifiers.Message)
        {
            string textMessage = csv[1];
            float duration = (csv.Length > 2 ? float.Parse(csv[2], CultureInfo.InvariantCulture) : 3.0f);
            int color = (csv.Length > 3 ? int.Parse(csv[3]) : 0);

            FeedbackManager.Instance.DisplayMessage(textMessage, duration, color);
        }
        else if (requestType == ServerToClientTransferSignifiers.SuccessfulLogin)
        {
            UIManager.Instance.SuccessfulLogin();
        }
        else if (requestType == ServerToClientTransferSignifiers.AddUserToLocalClient)
        {
            int userId = int.Parse(csv[1]);
            string name = csv[2];

            UsersManager.Instance.AddUser(userId,name);
        }
        else if (requestType == ServerToClientTransferSignifiers.UserDisconnected)
        {
            int userId = int.Parse(csv[1]);

            UsersManager.Instance.RemoveUser(userId);
        }
        else if (requestType == ServerToClientTransferSignifiers.ReceiveGlobalMessage)
        {
            int userId = int.Parse(csv[1]);
            string message = csv[2];

            ChatManager.Instance.ReceiveGlobalMessage(userId,message);
        }
        else if (requestType == ServerToClientTransferSignifiers.ReceivePrivateMessage)
        {
            int userId = int.Parse(csv[1]);
            string message = csv[2];

            ChatManager.Instance.ReceivePrivateMessage(userId, message);
        }
        else if (requestType == ServerToClientTransferSignifiers.AddMatchToList)
        {
            int matchId = int.Parse(csv[1]);
            string matchName = csv[2];

            MatchesManager.Instance.AddMatch(matchId,matchName);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}

public static class ClientToServerTransferSignifiers
{
    public const int CreateAccount = 1;
    public const int Login = 2;
    public const int ForgotPassword = 3;

    public const int SendGlobalMessage = 4;
    public const int SendPrivateMessage = 5;

    public const int CreateMatch = 6;
    public const int JoinMatch = 7;

    public const int GetMatchesList = 8;
}

public static class ServerToClientTransferSignifiers
{
    public const int Message = 1;
    public const int SuccessfulLogin = 2;

    public const int AddUserToLocalClient = 3;
    public const int UserDisconnected = 4;

    public const int ReceiveGlobalMessage = 5;
    public const int ReceivePrivateMessage = 6;

    public const int MatchStarted = 7;
    public const int MatchRemoved = 8;
    public const int MatchFinished = 9;

    public const int AddMatchToList = 10;
}