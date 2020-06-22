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


    // TEMP: we probably want to animate camera back to tray when finished
    public bool camSet;
    public Vector3 camDefaultPosition;
    public Vector3 camDefaultRotation;
    public float camDefaultFov;

    public SpecimenCart cart;

    void Start()
    {
        compareButton.onClick.AddListener(SelectCompare);
        analyzeButton.onClick.AddListener(SelectAnalysis);
        actionButtons.SetActive(false);
    }

    public void Activate() {
        uiObject.SetActive(true);

        // TEMP: use animation
        if (camSet)
        {
            Camera.main.transform.position = camDefaultPosition;
            Camera.main.transform.rotation = Quaternion.Euler(camDefaultRotation);
            Camera.main.fieldOfView = camDefaultFov;
        }

        // If no current specimen...
        if (ReferenceEquals(stateController.CurrentSpecimenData, null))
        {
            // ... hide the action buttons, turn off compare if relevant.
            actionButtons.SetActive(false);
            if (selectingCompareSpecimen) {
                CompareOff();
            }
        } else {
            // Else put compare mode on
            CompareOn();
        }

    }

    public void Deactivate() {
        uiObject.SetActive(false);
    }

    public void SpecimenSelected(SpecimenData data)
    {
        GameObject specimen;
        if (selectingCompareSpecimen)
        {
            specimen = stateController.AddCompareSpecimen(data);
            cart.AddSpecimenCompare(specimen);
        } else { 
            specimen = stateController.AddNewSpecimen(data);
            cart.AddSpecimenPrimary(specimen);
        }
        actionButtons.SetActive(true);

    }

    public void SelectAnalysis()
    {
        // TEMP: add animation later
        camDefaultPosition = Camera.main.transform.position;
        camDefaultRotation = Camera.main.transform.rotation.eulerAngles;
        camDefaultFov = Camera.main.fieldOfView;
        camSet = true;

        if (stateController.CompareSpecimenObject == null)
        {
            cart.RemoveTray2();
        }

        stateController.mode = ViewMode.ANALYSIS;
    }

    public void SelectCompare()
    {
        if (!selectingCompareSpecimen)
        {
            CompareOn();
        }
        else
        {
            CompareOff();
        }
    }

    private void CompareOff()
    {
        selectingCompareSpecimen = false;
        stateController.RemoveCompareSpecimen();
        compareButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Compare";
        selectorMenu.EndCompare();
        cart.RemoveTray2();
    }

    private void CompareOn()
    {
        selectingCompareSpecimen = true;
        selectorMenu.SelectCompare();
        compareButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Uncompare";
        cart.SpawnTray2();
    }
   


}
