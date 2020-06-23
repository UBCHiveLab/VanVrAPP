using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using TMPro;

public class AnnotationDisplay : MonoBehaviour
{
    public StateController stateController;
    public SpecimenData currentSpecimenData;
    public GameObject currentSpecimenObject;

    public List<AnnotationIndicator> activeIndicators = new List<AnnotationIndicator>();
    public AnnotationIndicator indicatorPrefab;

    void Start(){

    }

    public void Activate()
    {
        currentSpecimenData = stateController.CurrentSpecimenData;
        currentSpecimenObject = stateController.CurrentSpecimenObject;

        ClearAnnotations();
        DrawAnnotations();
    }

    void DrawAnnotations()
    {
        foreach (AnnotationData ad in currentSpecimenData.annotations)
        {
            AnnotationIndicator indicator = Instantiate(indicatorPrefab, transform);
            indicator.Populate(ad, currentSpecimenData, currentSpecimenObject);
        }
    }

    void ClearAnnotations()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    void OnMouseDown()
    {

    }

}
