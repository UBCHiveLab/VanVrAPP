using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class CompareMenu : MonoBehaviour
{
    [Header("Services")]
    public SpecimenStore store;
    public StateController stateController;
    public SpecimenCart cart;
    
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

        if (store == null) store = FindObjectOfType<SpecimenStore>();
        if (stateController == null) stateController = FindObjectOfType<StateController>();
    }

    public void TogglePanel() {
        gameObject.SetActive(!gameObject.activeSelf);
        viewToggle.onClick.RemoveAllListeners();
        viewToggle.onClick.AddListener(TogglePanel);

        if (gameObject.activeSelf)
        {
            cart.SpawnTray2();
        }
        else if (stateController.CompareSpecimenData == null)
        {
            cart.RemoveTray2();
        }
    }

    public void Populate()
    {
        Clear();

        if (stateController.CurrentSpecimenData == null)
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
        stateController.CurrentSpecimenObject.transform.SetParent(cart.tray1.transform);
        cart.ResetPosition(stateController.CurrentSpecimenObject);

        // If a specimen remains, populates the ui again.
        if (stateController.CurrentSpecimenData != null)
        {
            Populate();
            Camera.main.GetComponent<OrbitCamera>().target = stateController.CurrentSpecimenObject.transform;
        }
        else
        {
            cart.RemoveTray2();
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
        foreach (SpecimenData spd in store.GetSpecimenDataFiltered(new List<string>{stateController.currentSpecimenId, stateController.CompareSpecimenData?.id}))
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
