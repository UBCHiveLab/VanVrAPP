using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisclaimerPanel : MonoBehaviour
{
    [Header("Internal Structure")]
    public UITwoStateIndicator understoodIndicator;
    public HoverButton understoodHover;
    public Image understoodIcon;
    public TextMeshProUGUI understoodLabel;
    public UITwoStateIndicator enterIndicator;
    public GameObject arrowFrame;
    public Button enterButton;
    public GameObject warning;
    public Button backButton;
    public Animator anim;

    [Header("Actions")]
    public Action enterAction;
    public Action backAction;

    private bool understood;

    void Start()
    {
        enterButton.onClick.AddListener(TryEnter);
        backButton.onClick.AddListener(Back);
    }

    void OnEnable()
    {
        warning.SetActive(false);
        backButton.GetComponent<HoverButton>().OnPointerExit(null);
    }


    public void SetUnderstood(bool value)
    {
        warning.SetActive(false);
        understood = value;

        understoodHover.StopAllCoroutines();
        understoodHover.enabled = !understood;
        enterIndicator.UpdateState(understood);
        understoodIndicator.UpdateState(understood);

        if (value)
        {
            understoodIcon.color = Color.white;
            understoodLabel.color = understoodHover.baseColor;
        }
        else
        {
            understoodIcon.color = understoodHover.hoverColor;
            understoodLabel.color = understoodHover.hoverColor;
        }

        arrowFrame.SetActive(understood);


    }

    public void StartDismiss()
    {
        anim.SetTrigger("Dismiss");
    }

    public void TryEnter()
    {
        if (!understood)
        {
            warning.SetActive(true);
        }
        else
        {
            enterAction();
        }
        enterButton.OnDeselect(null);
    }

    public void Back()
    {
        backAction();
    }

    public void OnCompleteFade()
    {
        gameObject.SetActive(false);
    }
}
