using System.Collections.Generic;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompareMenu : MonoBehaviour
{
    [Header("Services")]
    public SpecimenStore store;
    public StateController stateController;
    public SpecimenCart cart;
    public ProportionIndicator proportionIndicator;
    public AnalysisPage analysisPage;
    
    [Header("Internal Structure")]
    public Button viewToggle;
    public SpecimenListing primarySpecimenListing;
    public SpecimenListing secondarySpecimenListing;
    public Transform addListTransform;
    public TextMeshProUGUI organNameLabel;

    [Header("Prefabs")]
    public SpecimenListing addListingPrefab;

    [Header("External Structure")] public GameObject leftPanel;

    void Start()
    {
        viewToggle.onClick.AddListener(TogglePanel);
    }

    void OnEnable()
    {
        Populate();

        if (store == null) store = FindObjectOfType<SpecimenStore>();
        if (stateController == null) stateController = FindObjectOfType<StateController>();

        viewToggle.onClick.RemoveAllListeners();
        viewToggle.onClick.AddListener(TogglePanel);
        leftPanel.SetActive(!gameObject.activeSelf);
        cart.SpawnTray2();
        
    }

    void OnDisable()
    {
        leftPanel.SetActive(!gameObject.activeSelf);
        if (stateController.CompareSpecimenData == null) {
            cart.RemoveTray2();
        }
    }

    public void TogglePanel() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Populate()
    {
        Clear();

        if (stateController.CurrentSpecimenData == null)
        {
            Debug.LogWarning("No specimen selected; not populating compare menu.");
            return;
        }

        organNameLabel.text = stateController.CurrentSpecimenData.organ;

        // Populates the primary specimen holder, including its "remove" button
        primarySpecimenListing.Populate(stateController.CurrentSpecimenData, () => {Remove(0);});

        // Populates the compare specimen holder, if there is a compare specimen selected, including its "remove" button
        if (stateController.CompareSpecimenData != null)
        {
            secondarySpecimenListing.gameObject.SetActive(true);
            secondarySpecimenListing.Populate(stateController.CompareSpecimenData, () => { Remove(1); });
        }
        else
        {
            secondarySpecimenListing.gameObject.SetActive(false);
        }

        // Generates the list of comparable specimens
        PopulateAddList();
    }

    public void Remove(int index)
    {
        if (index == 0)
        {
            stateController.RemoveCurrentSpecimen();
        }
        else
        {
            stateController.RemoveCompareSpecimen();
        }

        // If primary specimen is removed, compare specimen becomes the primary.
        secondarySpecimenListing.gameObject.SetActive(false);
        cart.AddSpecimenPrimary(stateController.CurrentSpecimenObject);
        cart.ResetPosition(stateController.CurrentSpecimenObject);

        // If a specimen remains, populates the ui again.
        if (stateController.CurrentSpecimenData != null)
        {
            Populate();
            analysisPage.ChangeFocus(stateController.CurrentSpecimenObject, stateController.CurrentSpecimenData);
        }
        else
        {
            cart.RemoveTray2();
        }

        proportionIndicator.HighlightProportionIndicator();
    }

    private void Clear()
    {
        foreach (Transform child in addListTransform) {
            Destroy(child.gameObject);
        }

        primarySpecimenListing.button.onClick.RemoveAllListeners();
        secondarySpecimenListing.button.onClick.RemoveAllListeners();
    }

    private void PopulateAddList()
    {
        // TODO: filter this by whatever we choose (maybe same organ?)

        // Filters given ids from specimen data list and populates those in the ui list.
        foreach (SpecimenData spd in store.GetSpecimenDataFiltered(new List<string>{stateController.currentSpecimenId, stateController.CompareSpecimenData?.id}))
        {
            SpecimenListing specimen = Instantiate(addListingPrefab, addListTransform);
            specimen.Populate(spd, () => {Add(spd);});

        }
    }

    public void Add(SpecimenData data)
    {
        StartCoroutine(stateController.AddCompareSpecimen(data, cart.AddSpecimenCompare));
        Populate();

        proportionIndicator.HighlightProportionIndicator();
    }

}
