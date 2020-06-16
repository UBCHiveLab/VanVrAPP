using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrayPage : MonoBehaviour, IPage
{
    public StateController stateController;
    public SelectorMenu selectorMenu;
    public GameObject uiObject;
    public GameObject actionButtons;
    public Button compareButton;
    public Button analyzeButton;
    public bool selectingCompareSpecimen;


    void Start()
    {
        compareButton.onClick.AddListener(SelectCompare);
        analyzeButton.onClick.AddListener(SelectAnalysis);
        actionButtons.SetActive(false);
    }

    public void Activate() {
        uiObject.SetActive(true);
    }

    public void Deactivate() {
        uiObject.SetActive(false);
    }

    public void SpecimenSelected(SpecimenData data)
    {
        if (selectingCompareSpecimen)
        {
            stateController.AddCompareSpecimen(data);
        }
        else
        {
            stateController.AddNewSpecimen(data);
        }
        actionButtons.SetActive(true);
    }

    public void SelectAnalysis()
    {
        stateController.mode = ViewMode.ANALYSIS;
    }

    public void SelectCompare()
    {
        if (!selectingCompareSpecimen)
        {
            selectingCompareSpecimen = true;
            selectorMenu.SelectCompare();
            compareButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Uncompare";
        }
        else
        {
            selectingCompareSpecimen = false;
            stateController.RemoveCompareSpecimen();
            compareButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Compare";
            selectorMenu.EndCompare();
        }


    }


}
