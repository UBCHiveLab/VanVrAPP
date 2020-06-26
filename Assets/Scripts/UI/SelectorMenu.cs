using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controller;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour
{

    private bool byLab;
    private string region = "";
    private string organ = "";
    private string labId = "";

    [Header("Services")]
    public SpecimenStore store;
    public TrayPage trayPage;

    [Header("Prefabs")]
    public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public LabOption labPrefab;

    [Header("Internal Structures")]
    public Transform listTransform;
    public TextMeshProUGUI title;
    public Button backButton;
    public TextMeshProUGUI subtitle;
    public Slider labToggle;

    public enum ListMode
    {
        REGION,
        REGION_EXPANDED,
        SPECIMEN,
        LAB,
        LAB_SPECIMENS
    }

    private List<string> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private bool _loading = true;
    private List<LabData> _loadedLabs;

    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();

        subtitle.text = "LOADING SPECIMENS...";
        labToggle.onValueChanged.AddListener((val) =>
        {
            byLab = Mathf.RoundToInt(val) == 1;
            organ = "";
            labId = "";
            region = "";
            Populate();
        });
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

            if (labId != "")
            {
                mode = ListMode.LAB_SPECIMENS;
                title.text = "SHELF";
                subtitle.text = store.labs[labId].labName;
                _loadedSpecimens = store.GetSpecimenDataForLab(labId);
            } 
            else
            {
                mode = ListMode.LAB;

                _loadedLabs = store.labs.Values.ToList();
            }
        }
        else if (region == "")
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.OrderBy(region => region.order).Select(region => region.name).ToList();
            mode = ListMode.REGION;
        }
        else if (organ == "")
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.OrderBy(region => region.order).Select(region => region.name).ToList();
            _loadedOrgans = store.specimensByRegionByOrgan[region].Keys.ToList();
            mode = ListMode.REGION_EXPANDED;
        }
        else
        {
            title.text = $"SPECIMEN LIST";
            subtitle.text = organ;
            _loadedRegions = store.GetSpecimensByRegionOrgan(region, organ).Select(x => x.id).ToList();
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

    public void LabSelected(string labId)
    {
        this.labId = labId;
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

        if (mode == ListMode.LAB)
        {
            subtitle.gameObject.SetActive(false);
            for (int i = 0; i < _loadedLabs.Count; i++)
            {
                LabOption lo = Instantiate(labPrefab, listTransform);
                lo.Populate(_loadedLabs[i], this);
            }

            return;
        }

        if (mode == ListMode.SPECIMEN || mode == ListMode.LAB_SPECIMENS)
        {
            // Activate and set shelf subtitle to current organ name
            subtitle.gameObject.SetActive(true);
            // TODO: deactivate toggles for labView, bodyView


            // Loops through all loaded specimens of organ type and produces a clickable button for each.
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                SelectorButton btn = Instantiate(lightSelectorPrefab, listTransform);
                btn.text.text = _loadedSpecimens[i].id;
                btn.indexValue = i;
                btn.button.onClick.AddListener(() => SelectSpecimen(_loadedSpecimens[btn.indexValue].id));
            }

            // Activates the back button, which takes user back to Region/Organ list
            backButton.onClick.AddListener(Back);
        }
        else
        {
            // Deactivates the subtitle
            subtitle.gameObject.SetActive(false);

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
        labId = "";
        Populate();
    }

    private void ToggleMenu()
    {
        Debug.Log("I'm toggglin' here");
    }
}
