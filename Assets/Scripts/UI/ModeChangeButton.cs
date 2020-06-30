using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class ModeChangeButton : MonoBehaviour
{
    public ViewMode mode;
    private StateController controller;

    // Start is called before the first frame update
    void Start()
    {
        
        if (controller == null) controller = FindObjectOfType<StateController>();
        GetComponent<Button>().onClick.AddListener(() => { controller.mode = mode; });

        
    }

}
