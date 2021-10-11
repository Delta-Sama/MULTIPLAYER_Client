using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] GameObject ChatCanvas;
    [SerializeField] GameObject LoginCanvas;

    void Start()
    {
        Instance = this;
    }

    public void SetChatActive(bool mode)
    {
        ChatCanvas.SetActive(mode);
    }

    public void SetLoginActive(bool mode)
    {
        LoginCanvas.SetActive(mode);
    }
}
