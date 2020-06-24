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
    public AnnotationDetailPanel detailPanel;

    void Start(){

    }

    public void Activate()
    {
        currentSpecimenData = stateController.CurrentSpecimenData;
        currentSpecimenObject = stateController.CurrentSpecimenObject;

        ClearAnnotations();
        DrawAnnotations();

        detailPanel.gameObject.SetActive(false);
    }

    void DrawAnnotations()
    {
        for (int i = 0; i < currentSpecimenData.annotations.Count; i++)
        {
            AnnotationData ad = currentSpecimenData.annotations[i];
            AnnotationIndicator indicator = Instantiate(indicatorPrefab, transform);
            indicator.Populate(i, ad, currentSpecimenData, currentSpecimenObject, this);
        }
    }

    void ClearAnnotations()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowDetail(AnnotationIndicator indicator)
    {
        detailPanel.Populate(currentSpecimenData.annotations[indicator.index], indicator);
        detailPanel.gameObject.SetActive(true);
    }

}
