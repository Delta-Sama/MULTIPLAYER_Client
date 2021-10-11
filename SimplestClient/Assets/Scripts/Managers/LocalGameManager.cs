using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager Instance;

    public Dictionary<int, UserAccount> connectedUsers;

    void Start()
    {
        Instance = this;

        connectedUsers = new Dictionary<int, UserAccount>();
    }

    public void AddUser(int userId, string name)
    {
        UserAccount user = new UserAccount();
        user.name = name;

        connectedUsers.Add(userId, user);

        Debug.Log("User connected: " + name + ", " + userId);
    }

    public void RemoveUser(int userId)
    {
        if (connectedUsers.ContainsKey(userId))
            connectedUsers.Remove(userId);
    }
}
