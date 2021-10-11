using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    void Start()
    {
        Instance = this;
    }

    public void ReceiveGlobalMessage(int userId, string message)
    {
        UserAccount user;
        LocalGameManager.Instance.connectedUsers.TryGetValue(userId, out user);

        Debug.Log("Global message from " + user.name + ": " + DecodeMessageToString(message));
    }

    public void ReceivePrivateMessage(int userId, string message)
    {
        UserAccount user;
        LocalGameManager.Instance.connectedUsers.TryGetValue(userId, out user);

        Debug.Log("Private message from " + user.name + ": " + DecodeMessageToString(message));
    }

    public string EncodeStringToMessage(string message)
    {
        string encodedMessage = message.Replace(',',';');

        return encodedMessage;
    }

    public string DecodeMessageToString(string message)
    {
        string decodedMessage = message.Replace(';', ',');

        return decodedMessage;
    }
}
