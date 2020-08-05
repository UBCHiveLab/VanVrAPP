using System.Collections.Generic;
using Assets.Scripts.State;
using UnityEngine;

/**
 * Manages the display and selection of current annotation indicators.
 */
public class AnnotationDisplay : MonoBehaviour {
    [Header("External Structures")]
    //public StateController stateController;
    public AnnotationDetailPanel detailPanel;
    public AnnotationSelector annotationSelector;
    public AnalysisPage analysisPage;

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
        SetFocus(analysisPage.currentSelectedObject, analysisPage.currentSelectedData);
        detailPanel.gameObject.SetActive(false);
        selectedSpecimenIndex = -1;
        annotationSelector.gameObject.SetActive(true);
    }

    public void OnDisable() {
        detailPanel.gameObject.SetActive(false);
        annotationSelector.gameObject.SetActive(false);
    }

    public void SetFocus(GameObject currentObject, SpecimenData currentData)
    {
        currentSpecimenData = currentData;
        currentSpecimenObject = currentObject;
        ClearAnnotations();
        if (currentObject != null)
        {
            DrawAnnotations();
        }

        ShowDetail(null);
    }

    /**
     * Populates the detail panel for the selected specimen
     */
    public void ShowDetail(AnnotationIndicator indicator) {
        if (indicator == null)
        {
            selectedSpecimenIndex = 0;
            detailPanel.Populate(null, null);
        } else
        {
            selectedSpecimenIndex = indicator.index;
            detailPanel.gameObject.SetActive(true);
            detailPanel.Populate(currentSpecimenData.annotations[indicator.index], indicator);
            annotationSelector.UpdateIndex();
        }
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

    public void SelectAnnotationIndex(int index)
    {
        if (activeIndicators.Count == 0 || activeIndicators.Count <= index) return;
        ShowDetail(activeIndicators[index]);
    }

    private void DrawAnnotations() {
        if (currentSpecimenData == null || currentSpecimenData.annotations == null) return;
        activeIndicators = new List<AnnotationIndicator>();
        for (int i = 0; i < currentSpecimenData.annotations.Count; i++) {
            AnnotationData ad = currentSpecimenData.annotations[i];
            AnnotationIndicator indicator = Instantiate(indicatorPrefab, transform);
            indicator.Populate(i, ad, currentSpecimenData, currentSpecimenObject, this);
            activeIndicators.Add(indicator);
        }
        annotationSelector.UpdateIndex();
        annotationSelector.PopulateDropUp();
    }

    private void ClearAnnotations() {
        selectedSpecimenIndex = -1;

        activeIndicators = new List<AnnotationIndicator>();
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

}