using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    public StateController controller;

    void OnEnable()
    {
        Debug.Log($"Opening help panel in {controller.mode} mode");
    }
}
