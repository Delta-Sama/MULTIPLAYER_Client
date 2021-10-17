using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UsersManager : MonoBehaviour
{
    public static UsersManager Instance;

    public UnityEvent<int> OnUserAddedEvent = new UnityEvent<int>();
    public UnityEvent<int> OnUserRemovedEvent = new UnityEvent<int>();

    public Dictionary<int, UserAccount> connectedUsers;

    private void Awake()
    {
        Instance = this;

        connectedUsers = new Dictionary<int, UserAccount>();
    }

    public void AddUser(int userId, string name)
    {
        UserAccount user = new UserAccount();
        user.name = name;
        user.userId = userId;

        connectedUsers.Add(userId, user);

        OnUserAddedEvent.Invoke(userId);

        Debug.Log("User connected: " + name + ", " + userId);
    }

    public UserAccount GetUser(int userId)
    {
        UserAccount user = null;
        UsersManager.Instance.connectedUsers.TryGetValue(userId, out user);

        return user;
    }

    public void RemoveUser(int userId)
    {
        if (connectedUsers.ContainsKey(userId))
            connectedUsers.Remove(userId);

        OnUserRemovedEvent.Invoke(userId);
    }
}
