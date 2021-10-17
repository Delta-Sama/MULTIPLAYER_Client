using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserButtonBehavior : MonoBehaviour
{
    private Image image;
    private Color SelectColor = new Color(1.0f,0.3f,0.3f);
    private Color StandardColor;

    void Start()
    {
        image = GetComponent<Image>();
        StandardColor = image.color;
    }

    public void SetActiveState(bool state)
    {
        if (state)
            image.color = SelectColor;
        else
            image.color = StandardColor;
    }
}
