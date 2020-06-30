using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using TMPro;

public class AnnotationDisplay : MonoBehaviour
{
    [Header("External Structures")]
    public StateController stateController;
    public AnnotationDetailPanel detailPanel;
    public AnnotationSelector annotationSelector;

    [Header("Data")]
    public SpecimenData currentSpecimenData;
    public GameObject currentSpecimenObject;
    public int selectedSpecimenIndex = -1;

    [Header("Internal Structures")]
    public List<AnnotationIndicator> activeIndicators = new List<AnnotationIndicator>();

    [Header("Prefabs")]
    public AnnotationIndicator indicatorPrefab;


    public void OnEnable()
    {
        currentSpecimenData = stateController.CurrentSpecimenData;
        currentSpecimenObject = stateController.CurrentSpecimenObject;

        ClearAnnotations();
        DrawAnnotations();

        detailPanel.gameObject.SetActive(false);
        annotationSelector.gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        detailPanel.gameObject.SetActive(false);
        annotationSelector.gameObject.SetActive(false);
    }

    /**
     * Populates the detail panel for the selected specimen
     */
    public void ShowDetail(AnnotationIndicator indicator) {
        selectedSpecimenIndex = indicator.index;
        detailPanel.Populate(currentSpecimenData.annotations[indicator.index], indicator);
        detailPanel.gameObject.SetActive(true);
        annotationSelector.UpdateIndex();
    }

    /**
     * For incrementing and decrementing the index of the current annotation
     */
    public void IncrementAnnotationIndex(int delta) {
        if (activeIndicators.Count == 0) return; // Can't divide by 0!
        selectedSpecimenIndex = (selectedSpecimenIndex + delta) % activeIndicators.Count;
        if (selectedSpecimenIndex < 0) selectedSpecimenIndex = activeIndicators.Count - 1;
        ShowDetail(activeIndicators[selectedSpecimenIndex]);
    }

    private void DrawAnnotations()
    {
        if (currentSpecimenData.annotations == null) return;
        activeIndicators = new List<AnnotationIndicator>();
        for (int i = 0; i < currentSpecimenData.annotations.Count; i++)
        {
            AnnotationData ad = currentSpecimenData.annotations[i];
            AnnotationIndicator indicator = Instantiate(indicatorPrefab, transform);
            indicator.Populate(i, ad, currentSpecimenData, currentSpecimenObject, this);
            activeIndicators.Add(indicator);
        }
    }

    private void ClearAnnotations()
    {
        selectedSpecimenIndex = -1;

        activeIndicators = new List<AnnotationIndicator>();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
