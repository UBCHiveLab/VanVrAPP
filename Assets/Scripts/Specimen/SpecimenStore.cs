using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SpecimenStore : MonoBehaviour
{
    public Dictionary<string, SpecimenData> specimens;
    public Dictionary<string, LabData> labs;
    public Dictionary<string, Dictionary<string, List<SpecimenData>>> specimensByRegionByOrgan;
    private bool _loading = true;

    public List<RegionData> regions;
    public Dictionary<string, RegionData> organToRegion;


    public List<string> GetSpecimenIdsList()
    {
        return specimens.Keys.ToList();
    }

    public SpecimenData GetSpecimen(string id)
    {
        if (!specimens.ContainsKey(id))
        {
            Debug.LogWarning($"No specimen found with id {id}");
            return null;
        }

        return specimens[id];
    }

    public Dictionary<string, List<SpecimenData>> GetSpecimensByRegion(string region)
    {
        if (!specimensByRegionByOrgan.ContainsKey(region))
        {
            Debug.LogWarning($"No specimen region found with id {region}");
            return null;
        }

        return specimensByRegionByOrgan[region];
    }


    public List<SpecimenData> GetSpecimensByRegionOrgan(string region, string organ)
    {
        if (!specimensByRegionByOrgan.ContainsKey(region))
        {
            Debug.LogWarning($"No specimen region found for {region}");
            return new List<SpecimenData>();
        }

        if (!specimensByRegionByOrgan.ContainsKey(region))
        {
            Debug.LogWarning($"No specimen organ found for {organ} in {region}");
            return new List<SpecimenData>();
        }

        return specimensByRegionByOrgan[region][organ];
    }

    public List<SpecimenData> GetSpecimenDataFiltered(List<string> filteredOutIds)
    {
        return specimens.Values.Where(spd => !filteredOutIds.Contains(spd.id)).ToList();
    }

    public bool Loading()
    {
        return _loading;
    }

    public string DumpCurrentSpecimenStructure()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Current Specimens: \n");
        foreach (var reg in specimensByRegionByOrgan)
        {
            sb.Append($"--{reg.Key}:\n");
            foreach (var org in reg.Value)
            {
                sb.Append($"----{org.Key}:\n");
                foreach (SpecimenData sd in org.Value)
                {
                    sb.Append($"------{sd.id}\n");
                }
            }
        }

        return sb.ToString();
    }

    private void Start()
    {
        StartCoroutine(LoadData());



    }

    private IEnumerator LoadData()
    {
        DataLoader loader = gameObject.AddComponent<LocalDataLoader>();
        loader.manifestPath = "manifest";
        loader.Load();

        while (!loader.Loaded()) yield return null;

        regions = loader.GetRegions().ToList();
        organToRegion = new Dictionary<string, RegionData>();
        foreach (RegionData region in regions)
        {
            foreach (string organ in region.organs)
            {
                organToRegion.Add(organ, region);
            }
        }

        labs = loader.GetLabs().ToDictionary((lab => lab.labId), lab => lab);
        specimens = loader.GetSpecimens().ToDictionary((spec => spec.id), spec => spec);
        specimensByRegionByOrgan = new Dictionary<string, Dictionary<string, List<SpecimenData>>>();

        
        foreach (SpecimenData spd in loader.GetSpecimens())
        {
            RegionData region = organToRegion[spd.organ];

            if (!specimensByRegionByOrgan.ContainsKey(region.name)) {
                specimensByRegionByOrgan.Add(region.name, new Dictionary<string, List<SpecimenData>>());
            }

            if (!specimensByRegionByOrgan[region.name].ContainsKey(spd.organ)) {
                specimensByRegionByOrgan[region.name].Add(spd.organ, new List<SpecimenData>());
            }

            specimensByRegionByOrgan[region.name][spd.organ].Add(spd);
        }

        _loading = false;


    }

}
