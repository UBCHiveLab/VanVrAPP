using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour
{

    private bool byLab;
    private string region = "";
    private string organ = "";

    [Header("Services")]
    public SpecimenStore store;
    public TrayPage trayPage;

    [Header("Prefabs")]
    public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;

    [Header("Internal Structures")]
    public Transform listTransform;
    public TextMeshProUGUI title;
    public Button backButton;
    public TextMeshProUGUI subtitle;

    public enum ListMode
    {
        REGION,
        REGION_EXPANDED,
        SPECIMEN,
        LAB
    }

    private List<string> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private bool _loading = true;

    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();

        subtitle.text = "LOADING SPECIMENS...";
    }

    void Update()
    {
        if (_loading && !store.Loading())
        {
            _loading = false;
            Populate();
        }
    }
    
    /**
     * Determines current population of the ui list and calls layout to output data.
     */
    public void Populate()
    {
        if (store.Loading()) return;

        ListMode mode;

        // Sets current list mode based on set fields.
        // Then prepares requested data:
        if (byLab)
        {
            _loadedRegions = new List<string>();
            // TODO
            mode = ListMode.LAB;
        }
        else if (region == "")
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.ToList();
            mode = ListMode.REGION;
        }
        else if (organ == "")
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.ToList();
            _loadedOrgans = store.specimensByRegionByOrgan[region].Keys.ToList();
            mode = ListMode.REGION_EXPANDED;
        }
        else
        {
            title.text = $"SPECIMEN LIST";
            _loadedRegions = store.GetSpecimensByRegionOrgan(region, organ).Select(x => x.Id).ToList();
            _loadedOrgans = store.specimensByRegionByOrgan[region].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region][organ];
            mode = ListMode.SPECIMEN;
        }

        Layout(mode);
    }


    public void SelectCompare()
    {
        region = "";
        organ = "";
        Populate();
    }

    public void EndCompare()
    {
        Populate();
    }

    /**
     * Lays out UI based on requested mode.
     * Called only through Populate() to ensure that correct data is available.
     */
    private void Layout(ListMode mode)
    {
        // Clears data.
        Clear();

        if (mode == ListMode.SPECIMEN)
        {
            // Activate and set shelf subtitle to current organ name
            subtitle.gameObject.SetActive(true);
            subtitle.text = organ;

            // TODO: deactivate toggles for labView, bodyView


            // Loops through all loaded specimens of organ type and produces a clickable button for each.
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                SelectorButton btn = Instantiate(lightSelectorPrefab, listTransform);
                btn.text.text = _loadedSpecimens[i].Id;
                btn.indexValue = i;
                btn.button.onClick.AddListener(() => SelectSpecimen(_loadedSpecimens[btn.indexValue].Id));
            }

            // Activates the back button, which takes user back to Region/Organ list
            backButton.onClick.AddListener(Back);
        }
        else
        {
            // Deactivates the subtitle
            subtitle.gameObject.SetActive(false);
            // TODO: activate toggles for labView, bodyView

            // Loops through loaded regions, producing a clickable button for each...
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransform);
                btn.text.text = _loadedRegions[i];
                btn.indexValue = i;

                // If a region is the currently selected, output the organs found as buttons below.
                if (_loadedRegions[i] == region)
                {
                    for (int j = 0; j < _loadedOrgans.Count; j++)
                    {
                        SelectorButton sbtn = Instantiate(lightSelectorPrefab, listTransform);
                        sbtn.text.text = _loadedOrgans[j];
                        sbtn.indexValue = j;
                        // Bind a click listener that loads the specimen selection view
                        sbtn.button.onClick.AddListener(() => { SelectOrgan(_loadedOrgans[sbtn.indexValue]);});
                    }

                    // Bind a click listener that closes the region accordion
                    btn.button.onClick.AddListener(UnselectRegion);
                }
                else
                {
                    // Bind a click listener that closes the current region accordion and opens a new one
                    btn.button.onClick.AddListener(() => { SelectRegion(_loadedRegions[btn.indexValue]); });
                }

            }

            // Bind a click listener that toggles the shelf menu
            backButton.onClick.AddListener(ToggleMenu);
        }


    }

    private void Clear()
    {
        // Clears all menu options
        foreach (Transform child in listTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectRegion(string region)
    {
        this.region = region;
        Populate();
    }

    private void SelectOrgan(string organ)
    {
        this.organ = organ;
        Populate();
    }

    private void SelectSpecimen(string specimenId)
    {
        trayPage.SpecimenSelected(store.specimens[specimenId]);
    }

    private void UnselectRegion()
    {
        region = "";
        Populate();
    }

    private void Back()
    {
        organ = "";
        Populate();
    }

    private void ToggleMenu()
    {
        Debug.Log("I'm toggglin' here");
    }
}
