using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour
{

    public bool byLab;
    public string region;
    public string organ;

    public StateController stateController;
    public SelectorButton selectorPrefab;
    public SelectorButton lightSelectorPrefab;
    public SpecimenStore store;
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
        if (stateController == null) stateController = FindObjectOfType<StateController>();
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

    public void Populate()
    {
        if (store.Loading()) return;

        ListMode mode;

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

    private void Layout(ListMode mode)
    {
        Clear();

        if (mode == ListMode.SPECIMEN)
        {
            subtitle.gameObject.SetActive(true);
            subtitle.text = organ;
            for (int i = 0; i < _loadedSpecimens.Count; i++)
            {
                SelectorButton btn = Instantiate(lightSelectorPrefab, listTransform);
                btn.text.text = _loadedSpecimens[i].Id;
                btn.indexValue = i;
                btn.button.onClick.AddListener(() => SelectSpecimen(_loadedSpecimens[btn.indexValue].Id));
            }

            backButton.onClick.AddListener(Back);
        }
        else
        {
            subtitle.gameObject.SetActive(false);
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                SelectorButton btn = Instantiate(selectorPrefab, listTransform);
                btn.text.text = _loadedRegions[i];
                btn.indexValue = i;

                if (_loadedRegions[i] == region)
                {
                    for (int j = 0; j < _loadedOrgans.Count; j++)
                    {
                        SelectorButton sbtn = Instantiate(lightSelectorPrefab, listTransform);
                        sbtn.text.text = _loadedOrgans[j];
                        sbtn.indexValue = j;
                        sbtn.button.onClick.AddListener(() => { SelectOrgan(_loadedOrgans[sbtn.indexValue]);});
                    }

                    btn.button.onClick.AddListener(UnselectRegion);
                }
                else
                {
                    btn.button.onClick.AddListener(() => { SelectRegion(_loadedRegions[btn.indexValue]); });
                }

            }
            backButton.onClick.AddListener(ToggleMenu);
        }


    }

    private void Clear()
    {
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
        stateController.AddNewSpecimen(store.specimens[specimenId]);
        stateController.mode = ViewMode.ANALYSIS;
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
