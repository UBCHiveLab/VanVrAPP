using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controller;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class CompareMenu : MonoBehaviour
{

    public SpecimenStore store;
    public StateController stateController;
    
    public Button viewToggle;
    public SpecimenListing primarySpecimenListing;
    public SpecimenListing secondarySpecimenListing;
    public Transform addListTransform;
    public SpecimenListing addListingPrefab;


    void OnEnable()
    {
        viewToggle.onClick.RemoveAllListeners();
        viewToggle.onClick.AddListener(TogglePanel);
        Populate();
    }


    public void TogglePanel() {

        gameObject.SetActive(!gameObject.activeSelf);
        Populate();
    }

    public void Populate()
    {
        Clear();
        Debug.Log(stateController.CurrentSpecimenData);
        primarySpecimenListing.Populate(stateController.CurrentSpecimenData, () => {Remove(0);});

        if (stateController.CompareSpecimenData != null)
        {
            secondarySpecimenListing.gameObject.SetActive(true);
            secondarySpecimenListing.Populate(stateController.CompareSpecimenData, () => { Remove(1); });
        }
        else
        {
            secondarySpecimenListing.gameObject.SetActive(false);
        }

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

        secondarySpecimenListing.gameObject.SetActive(false);

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
