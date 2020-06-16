using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class CompareMenu : MonoBehaviour
{
    [Header("Services")]
    public SpecimenStore store;
    public StateController stateController;
    
    [Header("Internal Structure")]
    public Button viewToggle;
    public SpecimenListing primarySpecimenListing;
    public SpecimenListing secondarySpecimenListing;
    public Transform addListTransform;

    [Header("Prefabs")]
    public SpecimenListing addListingPrefab;


    void OnEnable()
    {
        Populate();
    }

    public void TogglePanel() {
        gameObject.SetActive(!gameObject.activeSelf);
        viewToggle.onClick.RemoveAllListeners();
        viewToggle.onClick.AddListener(TogglePanel);
    }

    public void Populate()
    {
        Clear();

        if (stateController.CompareSpecimenData == null)
        {
            Debug.LogWarning("No specimen selected; not populating compare menu.");
            return;
        }

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

        // If a specimen remains, populates the ui again.
        if (stateController.CurrentSpecimenData != null)
        {
            Populate();
        }
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
        foreach (SpecimenData spd in store.GetSpecimenDataFiltered(new List<string>{stateController.currentSpecimenId, stateController.CompareSpecimenData?.Id}))
        {
            SpecimenListing specimen = Instantiate(addListingPrefab, addListTransform);
            specimen.Populate(spd, () => {Add(spd);});

        }
    }

    public void Add(SpecimenData data)
    {
        stateController.AddCompareSpecimen(data);
        Populate();
    }

}
