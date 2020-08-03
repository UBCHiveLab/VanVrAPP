using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    public TextMeshProUGUI messageLabel;
    public Button yesButton;
    public Button noButton;


    private Action yes;
    private Action no;

    public void Start()
    {
        yesButton.onClick.AddListener(ChooseYes);
        noButton.onClick.AddListener(ChooseNo);
    }

    public void Populate(string text, Action noAction, Action yesAction)
    {
        gameObject.SetActive(true);
        yes = yesAction;
        no = noAction;
        messageLabel.text = text;

    }

    public void ChooseYes()
    {
        gameObject.SetActive(false);
        yes();
    }

    public void ChooseNo()
    {
        gameObject.SetActive(false);
        no?.Invoke();
    }
}
