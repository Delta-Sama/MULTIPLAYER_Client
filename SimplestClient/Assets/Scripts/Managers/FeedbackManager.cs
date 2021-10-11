using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;

    [SerializeField] GameObject message;

    private float displayingMessageTime;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (message.activeSelf && displayingMessageTime < Time.time)
            message.SetActive(false);
    }

    private Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue };
    public void DisplayMessage(string textMessage, float duration, int color = 0)
    {
        message.SetActive(true);
        message.GetComponent<Text>().text = textMessage;
        message.GetComponent<Text>().color = colors[color];

        displayingMessageTime = Time.time + duration;
    }
}
