using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Populates and manages the “Shelf” menu in Tray mode.
 */
public class SelectorMenu : MonoBehaviour
{
    private bool byLab;
    private RegionData region = null;
    private string organ = "";
    private string labId = "";

    [Header("Services")] public SpecimenStore store;
    public TrayPage trayPage;
    public StateController stateController;

    [Header("Prefabs")] public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public LabOption labPrefab;
    public SelectorButton noSpecimensPrefab;
    public SelectorButton specimenSelectorPrefab;
    public Button seeAllButtonPrefab;

    [Header("Internal Structures")] public Transform listTransform;
    public TextMeshProUGUI title;
    public Button backButton;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI labtitle;
    public GameObject labAtlasToggle;
    public Button atlasButton;
    public Button labButton;
    public TextMeshProUGUI atlasLabel;
    public TextMeshProUGUI labLabel;
    public GameObject loadingIndicator;
    public Animator anim;

    private ListMode currentMode;
    private Dictionary<string, SelectorButton> idToButton = new Dictionary<string, SelectorButton>();

    public enum ListMode
    {
        REGION,
        REGION_EXPANDED,
        SPECIMEN,
        LAB,
        LAB_SPECIMENS
    }


    private List<RegionData> _loadedRegions;
    private List<string> _loadedOrgans;
    private List<SpecimenData> _loadedSpecimens;
    private bool _loading = true;
    private List<LabData> _loadedLabs;

    void Start()
    {
        if (store == null) store = FindObjectOfType<SpecimenStore>();


        subtitle.text = "LOADING SPECIMENS...";
        labButton.onClick.AddListener(ToggleToLabs);
        atlasButton.onClick.AddListener(ToggleToAtlas);

        ToggleToAtlas();
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
        subtitle.gameObject.SetActive(false);

        if (store.Loading())
        {
            loadingIndicator.gameObject.SetActive(true);
            return;
        }

        loadingIndicator.gameObject.SetActive(false);

        ListMode mode;

        // Sets current list mode based on set fields.
        // Then prepares requested data:
        if (byLab)
        {
            if (labId != "")
            {
                mode = ListMode.LAB_SPECIMENS;
                title.text = "SHELF";
                _loadedSpecimens = store.GetSpecimenDataForLab(labId);
                labAtlasToggle.SetActive(false);
                labtitle.gameObject.SetActive(true);
                labtitle.text = store.labs[labId].labName;
            }
            else
            {
                mode = ListMode.LAB;
                _loadedLabs = store.labs.Values.ToList();
                labAtlasToggle.SetActive(true);
                labtitle.gameObject.SetActive(false);
            }
        }
        else if (region == null)
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            mode = ListMode.REGION;
            labAtlasToggle.SetActive(true);
            labtitle.gameObject.SetActive(false);
            subtitle.gameObject.SetActive(false);
        }
        else if (string.IsNullOrEmpty(organ))
        {
            title.text = $"SHELF";
            _loadedRegions = store.regions.OrderBy(r => r.order).ToList();
            _loadedOrgans = store.GetOrgansByRegion(region.name);
            mode = ListMode.REGION_EXPANDED;
            labAtlasToggle.SetActive(true);
            labtitle.gameObject.SetActive(false);
            subtitle.gameObject.SetActive(false);
        }
        else
        {
            title.text = $"SPECIMEN LIST";
            subtitle.text = organ;
            //_loadedRegions = store.GetSpecimensByRegionOrgan(region.name, organ).Select(x => x.id).ToList();
            _loadedOrgans = store.specimensByRegionByOrgan[region.name].Keys.ToList();
            _loadedSpecimens = store.specimensByRegionByOrgan[region.name][organ];
            mode = ListMode.SPECIMEN;
            labAtlasToggle.SetActive(false);
            labtitle.gameObject.SetActive(false);
            subtitle.gameObject.SetActive(true);
        }

        Layout(mode);
    }


    public void SelectCompare()
    {
        region = null;
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
        idToButton = new Dictionary<string, SelectorButton>();
        currentMode = mode;

        if (mode == ListMode.LAB)
        {
            subtitle.gameObject.SetActive(false);
            for (int i = 0; i < _loadedLabs.Count; i++)
            {
                LabOption lo = Instantiate(labPrefab, listTransform);
                lo.Populate(_loadedLabs[i], this);
            }

            backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
            return;
        }

        if (mode == ListMode.SPECIMEN || mode == ListMode.LAB_SPECIMENS)
        {
            // Forgive me for the spaghetti below
            // Loops through all loaded specimens of organ type and produces a clickable button for each.
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                string id = _loadedSpecimens[i].id;
                SelectorButton btn = Instantiate(specimenSelectorPrefab, listTransform);
                btn.Populate(_loadedSpecimens[i].name, i, null);
                idToButton.Add(id, btn);
            }

            if (stateController.CurrentSpecimenData != null && trayPage.selectingCompareSpecimen)
            {
                Button btn = Instantiate(seeAllButtonPrefab, listTransform);
                btn.onClick.AddListener(Back);
            }

            // Activates the back button, which takes user back to Region/Organ list
            backButton.onClick.AddListener(Back);
            UpdateSelected();
        }
        else
        {
            subtitle.gameObject.SetActive(false);

            // Loops through loaded regions, producing a clickable button for each...
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransform);
                btn.Populate(_loadedRegions[i].name, i, _loadedRegions[i].icon);
                idToButton.Add(_loadedRegions[i].name, btn);

                // If a region is the currently selected, output the organs found as buttons below.
                if (_loadedRegions[i] == region)
                {
                    btn.ShowBackground(true);
                    btn.children.gameObject.SetActive(true);

                    if (_loadedOrgans.Count == 0)
                    {
                        SelectorButton sbtn = Instantiate(noSpecimensPrefab, btn.children);
                        sbtn.button.onClick.AddListener(UnselectRegion);
                    }
                    else
                    {
                        for (int j = 0; j < _loadedOrgans.Count; j++)
                        {
                            SelectorButton sbtn = Instantiate(lightSelectorPrefab, btn.children);
                            sbtn.Populate(_loadedOrgans[j], j, null);


                            // Bind a click listener that loads the specimen selection view
                            sbtn.button.onClick.AddListener(() => { SelectOrgan(_loadedOrgans[sbtn.indexValue]); });
                            idToButton.Add(_loadedOrgans[sbtn.indexValue], sbtn);
                        }
                    }

                    // Bind a click listener that closes the region accordion
                    btn.button.onClick.AddListener(UnselectRegion);
                }
                else
                {
                    btn.children.gameObject.SetActive(false);
                    // Bind a click listener that closes the current region accordion and opens a new one
                    btn.button.onClick.AddListener(() => { SelectRegion(_loadedRegions[btn.indexValue]); });
                }
            }

            // Bind a click listener that toggles the shelf menu
            backButton.onClick.AddListener(trayPage.ToggleShelfMenu);
        }
    }

    private void Clear()
    {
        backButton.onClick.RemoveAllListeners();

        // Clears all menu options
        foreach (Transform child in listTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectRegion(RegionData region)
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
        SetSpecimenButtonToSelected(specimenId);
        trayPage.SpecimenSelected(store.specimens[specimenId]);
    }

    private void UnselectRegion()
    {
        region = null;
        Populate();
    }

    private void Back()
    {
        organ = "";
        labId = "";
        Populate();
    }

    private void ToggleToAtlas()
    {
        labButton.interactable = true;
        atlasButton.interactable = false;
        atlasLabel.color = Color.white;
        labLabel.color = Color.black;
        byLab = false;
        organ = "";
        labId = "";
        region = null;
        Populate();
    }

    private void ToggleToLabs()
    {
        labButton.interactable = false;
        atlasButton.interactable = true;
        labLabel.color = Color.white;
        atlasLabel.color = Color.black;
        organ = "";
        labId = "";
        region = null;
        byLab = true;
        Populate();
    }

    private void SetSpecimenButtonToSelected(string specId)
    {
        if (!idToButton.ContainsKey(specId)) return; //Current specimen not on screen
        SelectorButton btn = idToButton[specId];
        btn.ShowBackground(true);
        btn.icon.gameObject.SetActive(true);
        btn.button.onClick.RemoveAllListeners();
        btn.button.onClick.AddListener(() =>
        {
            trayPage.RemoveEitherActiveSpecimen(specId);
            UpdateSelected();
        });
    }

    private void SetSpecimenButtonToDeselected(string specId)
    {
        if (!idToButton.ContainsKey(specId)) return; //Current specimen not on screen
        SelectorButton btn = idToButton[specId];
        btn.ShowBackground(false);
        btn.icon.gameObject.SetActive(false);
        btn.button.onClick.RemoveAllListeners();
        btn.button.onClick.AddListener(() => {
            SelectSpecimen(_loadedSpecimens[btn.indexValue].id);
            btn.SetLoadingUntil(() => store.specimens[_loadedSpecimens[btn.indexValue].id].dataLoaded);
            UpdateSelected();
        });
    }

    public void SetOrganRegion(string organId, RegionData regionData)
    {
        organ = organId;
        region = regionData;
        Populate();
    }

    // Scans all active selector buttons and sets them to active if they are a selected specimen
    public void UpdateSelected()
    {
        if (currentMode != ListMode.SPECIMEN && currentMode != ListMode.LAB_SPECIMENS) return;
        string primaryId = null;
        string compareId = null;

        if (stateController.currentSpecimenId != null) {
            primaryId = stateController.currentSpecimenId;
        }
        if (stateController.CompareSpecimenData != null) {
            compareId = stateController.CompareSpecimenData.id;
        }

        foreach (string key in idToButton.Keys)
        {
            if (key == primaryId || key == compareId)
            {
                SetSpecimenButtonToSelected(key);
            }
            else
            {
                SetSpecimenButtonToDeselected(key);
            }
        }
    }

    // Called by EventTrigger on object
    public void HoverShelfToggle()
    {
        anim.SetBool("PeekMenu", true);
    }

    public void UnhoverShelfToggle()
    {
        anim.SetBool("PeekMenu", false);
    }
}