using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour
{
    Keyboard keyboard;
    TextMeshProUGUI buttonText;

    void Start()
    {
        keyboard = GetComponentInParent<Keyboard>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text.Length == 1)
        {
            NameToButtonText();
            GetComponent<Button>().onClick.AddListener(delegate { keyboard.InsertChar(buttonText.text); });
        }
    }

    public void NameToButtonText()
    {
        buttonText.text = gameObject.name;
    }

}
