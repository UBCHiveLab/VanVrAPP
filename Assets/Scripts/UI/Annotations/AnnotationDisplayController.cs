using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationDisplayController : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject displayOn;
    public GameObject displayOff;
    public GameObject annotationDisplayControllerUI;

    private AnnotationIndicator annotationIndicator;

    public bool isAnnotationDisplayOn;
    public bool isButtonOn;

    void Start()
    {
        isButtonOn = true;
        isAnnotationDisplayOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnnotationDisplayControllerStatement()
    {
        annotationDisplayControllerUI.SetActive(!isAnnotationDisplayOn);
        isAnnotationDisplayOn = !isAnnotationDisplayOn;
    }

    public void AnnotationDisplayControllerButton()
    {
        isButtonOn = !isButtonOn;
        displayOff.SetActive(!isButtonOn);
        displayOn.SetActive(isButtonOn);
    }
}
