﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    public Button interactable;
    public bool on;
    public Animator anim;

    public Action<bool> toggleFunc;

    void OnEnable()
    {
        anim.SetBool("On", on);
    }

    public void Bind(Action<bool> tFunc)
    {
        toggleFunc = tFunc;
        interactable.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        on = !on;
        toggleFunc?.Invoke(on);
        anim.SetTrigger("Change");
        anim.SetBool("On", on);
    }

}
