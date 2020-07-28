using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    private Action buttonAction;

    private void OnMouseDown()
    {
        buttonAction();
    }

    public void bindAction(Action action)
    {
        buttonAction = action;
    }
}
