using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Canvases")]
    [SerializeField] GameObject GameCanvas;
    [SerializeField] GameObject MenuCanvas;
    [SerializeField] GameObject ChatCanvas;
    [SerializeField] GameObject LoginCanvas;

    void Start()
    {
        Instance = this;
    }

    public void SuccessfulLogin()
    {
        LoginCanvas.SetActive(false);
        MenuCanvas.SetActive(true);
    }

    public void OnReturnToMenuButtonClick()
    {
        ChatCanvas.SetActive(false);
        GameCanvas.SetActive(false);

        MenuCanvas.SetActive(true);
    }

    public void OnChatButtonClick()
    {
        MenuCanvas.SetActive(false);
        ChatCanvas.SetActive(true);
    }

    public void OnGameButtonClick()
    {
        MenuCanvas.SetActive(false);
        GameCanvas.SetActive(true);
    }

    public void OnRefreshButtonClick()
    {
        MatchesManager.Instance.RefreshMatchList();
    }
}
