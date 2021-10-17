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
    [SerializeField] private GameObject OwnGlobalMessagePrefab;
    [SerializeField] private GameObject OwnPrivateMessagePrefab;
    [SerializeField] private GameObject GlobalMessagePrefab;
    [SerializeField] private GameObject PrivateMessagePrefab;
    [SerializeField] private GameObject UserButtonPrefab;

    private Queue<GameObject> messagesQueue;
    private Dictionary<int, GameObject> userButtons;

    private int privateRecieverId = -1;
    private bool isPrivateMessage { get => privateRecieverId >= 0; }

    private void Awake()
    {
        Instance = this;

        messagesQueue = new Queue<GameObject>();
        userButtons = new Dictionary<int, GameObject>();

        SendButton.onClick.AddListener(SendMessage);
    }

    void Start()
    {
        UsersManager.Instance.OnUserAddedEvent.AddListener(AddUserButton);
        UsersManager.Instance.OnUserRemovedEvent.AddListener(RemoveUserButton);
    }

    private void SendMessage()
    {
        if (Input.text.Length == 0) return;

        string message = Input.text;

        if (isPrivateMessage)
        {
            NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.SendPrivateMessage + "," + privateRecieverId + "," + message);

            UserAccount user = UsersManager.Instance.GetUser(privateRecieverId);
            AddMessage(message, "To: " + user.name, MessageType.OwnPrivateMessage);
        }
        else
        {
            NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.SendGlobalMessage + "," + message);

            AddMessage(message, "Self", MessageType.OwnGlobalMessage);
        }

        // Clear input
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
        if (type == MessageType.OwnGlobalMessage)
            Prefab = OwnGlobalMessagePrefab;
        else if (type == MessageType.OwnPrivateMessage)
            Prefab = OwnPrivateMessagePrefab;
        else if (type == MessageType.PrivateMessage)
            Prefab = PrivateMessagePrefab;

        GameObject Message = Instantiate(Prefab, ChatContent.transform);
        Message.GetComponentInChildren<Text>().text = message;
        Message.transform.Find("Name").GetComponent<Text>().text = name;
        Message.transform.SetAsFirstSibling();

        messagesQueue.Enqueue(Message);
    }

    public void AddUserButton(int userId)
    {
        UserAccount user = UsersManager.Instance.GetUser(userId);
        GameObject UserButton = Instantiate(UserButtonPrefab, UsersContent.transform);
        UserButton.transform.Find("Name").GetComponent<Text>().text = user.name;

        UserButton.GetComponent<Button>().onClick.AddListener(() => UserButtonClick(user));

        userButtons.Add(userId, UserButton);
    }

    private void UserButtonClick(UserAccount user)
    {
        if (privateRecieverId != user.userId)
            privateRecieverId = user.userId;
        else
            privateRecieverId = -1;

        SetButtonsState();
    }

    private void SetButtonsState()
    {
        // Set visual states for user buttons
        foreach (var userButton in userButtons)
        {
            userButton.Value.GetComponent<UserButtonBehavior>().SetActiveState(userButton.Key == privateRecieverId);
        }

        // Set visual state for send button
        SendButton.GetComponent<UserButtonBehavior>().SetActiveState(isPrivateMessage);
    }

    public void RemoveUserButton(int userId)
    {
        GameObject UserButton;
        if (userButtons.TryGetValue(userId, out UserButton))
        {
            Destroy(UserButton);
        }

        if (userId == privateRecieverId)
        {
            privateRecieverId = 0;
            SetButtonsState();
        }
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

        AddMessage(message, user.name, MessageType.PrivateMessage);
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
    OwnGlobalMessage,
    OwnPrivateMessage,
    GlobalMessage,
    PrivateMessage
}