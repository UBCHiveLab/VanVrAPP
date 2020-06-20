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


    public Transform trayParent;
    public GameObject trayPrefab;
    public Vector3 singleTrayLocalOffset = new Vector3(-0.25f, 2.25f, 0.5f);
    public Vector3 compareTray1LocalOffset = new Vector3(-0.75f, 2.25f, 0.5f);
    public Vector3 compareTray2LocalOffset = new Vector3(0.5f, 2.25f, 0.5f);
    public GameObject tray1;
    public GameObject tray2;

    public Vector3 specimenTrayOffset = new Vector3(0, 0, 0.005f);

    void Start()
    {
        compareButton.onClick.AddListener(SelectCompare);
        analyzeButton.onClick.AddListener(SelectAnalysis);
        actionButtons.SetActive(false);
    }

    public void Activate() {
        uiObject.SetActive(true);
        SpawnTray1();
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
            specimen.transform.SetParent(tray2.transform);
            specimen.transform.localPosition = specimenTrayOffset;
        } else { 
            specimen = stateController.AddNewSpecimen(data);
            specimen.transform.SetParent(tray1.transform);
            specimen.transform.localPosition = specimenTrayOffset;
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
            SpawnTray2();
        }
        else
        {
            selectingCompareSpecimen = false;
            stateController.RemoveCompareSpecimen();
            compareButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Compare";
            selectorMenu.EndCompare();
            RemoveTray2();
        }


    }


    public void SpawnTray1()
    {
        if (tray1 != null) return;
        tray1 = Instantiate(trayPrefab, trayParent);
        if (tray2 == null) {
            tray1.transform.localPosition = singleTrayLocalOffset;
        } else {
            tray1.transform.localPosition = compareTray1LocalOffset;
        }
    }

    public void SpawnTray2() {
        tray2 = Instantiate(trayPrefab, trayParent);
        tray2.transform.localPosition = compareTray2LocalOffset;
        tray1.transform.localPosition = compareTray1LocalOffset;
    }

    public void RemoveTray1()
    {
        Destroy(tray1);
        tray1 = tray2;
        tray1.transform.localPosition = singleTrayLocalOffset;
        tray2 = null;
    }

    public void RemoveTray2() {
        Destroy(tray2);
        tray2 = null;
        tray1.transform.localPosition = singleTrayLocalOffset;
    }


}
