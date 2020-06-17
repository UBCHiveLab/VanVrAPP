using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnnotationDisplay : MonoBehaviour
{
    [Header("--- Annotation Text ---")]
    public TextMeshProUGUI annotationText;

    void Start(){
        // Setting text blank at the start
        TextMeshProUGUI annotationTextUI = annotationText.GetComponent<TextMeshProUGUI>();
        annotationTextUI.text = " ";
    }

    
    void OnMouseDown()
    {
        // We will have to fetch data from each specimen object for:
        // 1. Name of Specimen
        // 2. Part of body it belongs to
        // 3. Rich Media Links: Could be a button that loads a specific video on the TV
        
        TextMeshProUGUI annotationTextUI = annotationText.GetComponent<TextMeshProUGUI>();
        Debug.Log(name);
        annotationTextUI.text = "Text description about " + name.ToString() + " goes over here. ";

    }
}
