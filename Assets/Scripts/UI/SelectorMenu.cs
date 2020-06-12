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
    public SpecimenStore store;
    public Transform listTransform;
    public TextMeshProUGUI title;

    public enum ListType
    {
        REGION, ORGAN, SPECIMEN, LAB
    }

    private List<string> _currentValues;
    private bool _loading = true;

    void Start()
    {
        if (stateController == null) stateController = FindObjectOfType<StateController>();
        if (store == null) store = FindObjectOfType<SpecimenStore>();

        title.text = "Loading...";
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

        ListType type;

        if (byLab)
        {
            _currentValues = new List<string>();
            // TODO
            type = ListType.LAB;
        } else if (region == "")
        {
            title.text = $"Choose a region";
            _currentValues = store.regions.ToList();
            type = ListType.REGION;
        } else if (organ == "")
        {
            title.text = $"Organs for: {region}";
            _currentValues = store.specimensByRegionByOrgan[region].Keys.ToList();
            type = ListType.ORGAN;
        }
        else
        {
            title.text = $"Specimens for: {organ}";
            _currentValues = store.GetSpecimensByRegionOrgan(region, organ).Select(x => x.Id).ToList();
            type = ListType.SPECIMEN;
        }

        Layout(_currentValues, type);

    }

    private void Layout(List<string> values, ListType type)
    {
        Clear();
        for (int i = 0; i < values.Count; i++)
        {
            SelectorButton btn = Instantiate(selectorPrefab, listTransform);
            btn.text.text = values[i];
            btn.indexValue = i;
            btn.button.onClick.AddListener(() =>
            {
                Selected(btn.indexValue, type);
            });
        }

        if (type != ListType.REGION)
        {
            SelectorButton back = Instantiate(selectorPrefab, listTransform);
            back.text.text = "<- Back";
            back.button.onClick.AddListener(() => { Back(type); });
        }


    }

    private void Clear()
    {
        foreach (Transform child in listTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Selected(int index, ListType type)
    {
        switch (type)
        {
            case ListType.REGION:
                region = _currentValues[index];
                Populate();
                break;
            case ListType.ORGAN:
                organ = _currentValues[index];
                Populate();
                break;
            case ListType.SPECIMEN:
                stateController.AddNewSpecimen(store.specimens[_currentValues[index]]);
                stateController.mode = ViewMode.ANALYSIS;
                break;
            case ListType.LAB:
                // TODO: lab view
                break;
            default:
                break;
        }
    }

    private void Back(ListType type)
    {
        switch (type)
        {
            case ListType.ORGAN:
                region = "";
                Populate();
                break;
            case ListType.SPECIMEN:
                organ = "";
                Populate();
                break;
        }
    }
}
