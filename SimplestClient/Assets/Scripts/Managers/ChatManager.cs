using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [Header("UI Fields")]
    [SerializeField] private InputField Input;
    [SerializeField] private Button SendButton;
    [SerializeField] private GameObject ChatContent;
    [SerializeField] private GameObject UsersContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject OwnMessagePrefab;
    [SerializeField] private GameObject GlobalMessagePrefab;
    [SerializeField] private GameObject PrivateMessagePrefab;
    [SerializeField] private GameObject UserButtonPrefab;

    private Queue<GameObject> messagesQueue;

    void Start()
    {
        Instance = this;

        messagesQueue = new Queue<GameObject>();

        SendButton.onClick.AddListener(SendMessage);
    }

    private void SendMessage()
    {
        if (Input.text.Length == 0) return;

        string message = Input.text;
        NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.SendGlobalMessage + "," + message);

        AddMessage(message, "Self", MessageType.OwnMessage);

        Input.Select();
        Input.text = "";
    }

    public void AddMessage(string message, string name, MessageType type = MessageType.GlobalMessage)
    {
        if (messagesQueue.Count >= 8)
        {
            GameObject OldestMessage = messagesQueue.Dequeue();
            Destroy(OldestMessage);
        }

        GameObject Prefab = GlobalMessagePrefab;
        if (type == MessageType.OwnMessage)
            Prefab = OwnMessagePrefab;
        else if (type == MessageType.PrivateMessage)
            Prefab = PrivateMessagePrefab;

        GameObject Message = Instantiate(Prefab, ChatContent.transform);
        Message.GetComponentInChildren<Text>().text = message;
        Message.transform.Find("Name").GetComponent<Text>().text = name;
        Message.transform.SetAsFirstSibling();

        messagesQueue.Enqueue(Message);
    }

    public void ReceiveGlobalMessage(int userId, string message)
    {
        UserAccount user = UsersManager.Instance.GetUser(userId);

        Debug.Log("Global message from " + user.name + ": " + DecodeMessageToString(message));

        AddMessage(message, user.name);
    }

    public void ReceivePrivateMessage(int userId, string message)
    {
        UserAccount user = UsersManager.Instance.GetUser(userId);

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

public enum MessageType
{
    OwnMessage,
    GlobalMessage,
    PrivateMessage
}