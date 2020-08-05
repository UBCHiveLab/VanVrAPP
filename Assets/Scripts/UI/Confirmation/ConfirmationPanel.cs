using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    [Header("Internal Structure")]
    public TextMeshProUGUI messageLabel;
    public TextMeshProUGUI titleLabel;
    public Button yesButton;
    public Button noButton;

    private Action yes;
    private Action no;

    public void Start()
    {
        yesButton.onClick.AddListener(ChooseYes);
        noButton.onClick.AddListener(ChooseNo);
    }

    public void Populate(string text, string title, Action noAction, Action yesAction)
    {
        gameObject.SetActive(true);
        yes = yesAction;
        no = noAction;
        messageLabel.text = text;
        titleLabel.text = title;
    }

    public void ChooseYes()
    {
        gameObject.SetActive(false);
        yes?.Invoke();
    }

    public void ChooseNo()
    {
        gameObject.SetActive(false);
        no?.Invoke();
    }
}
