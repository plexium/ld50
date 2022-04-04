using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogBox : MonoBehaviour
{
    public Text textTitle;
    public Text textMessage;
    public Image image;

    public string title;
    public string message;
    public Sprite icon;

    void Update()
    {
        textTitle.text = title;
        textMessage.text = message;
        if (icon != null) image.sprite = icon;
    }
}
