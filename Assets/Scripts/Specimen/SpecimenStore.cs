using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SpecimenStore : MonoBehaviour
{

    public Dictionary<string, SpecimenData> specimens = new Dictionary<string, SpecimenData>();
    public Dictionary<string, Dictionary<string, List<SpecimenData>>> specimensByRegionByOrgan = new Dictionary<string, Dictionary<string, List<SpecimenData>>>();
    public string[] regions = {"Head", "Thorax", "Abdomen"};
    public string[] organs = { "brain", "heart", "kidney", "liver", /*"right lung",*/ "skull" };
    public Dictionary<string, string> organToRegion = new Dictionary<string, string>
    {
        {"brain", "Head"},
        {"heart", "Thorax"},
        {"kidney", "Abdomen"},
        {"liver", "Abdomen"},
        {"skull", "Head"}
    };

    private bool _loading = true;

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
        if (!specimensByRegionByOrgan.ContainsKey(region)) {
            Debug.LogWarning($"No specimen region found with id {region}");
            return null;
        }

        return specimensByRegionByOrgan[region];
    }


    public List<SpecimenData> GetSpecimensByRegionOrgan(string region, string organ) {
        if (!specimensByRegionByOrgan.ContainsKey(region)) {
            Debug.LogWarning($"No specimen region found for {region}");
            return new List<SpecimenData>();
        }

        if (!specimensByRegionByOrgan.ContainsKey(region)) {
            Debug.LogWarning($"No specimen organ found for {organ} in {region}");
            return new List<SpecimenData>();
        }

        return specimensByRegionByOrgan[region][organ];
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
                    sb.Append($"------{sd.Id}\n");
                }
            }
        }

        return sb.ToString();
    }


    private void Start() {
        StartCoroutine(GetAssetBundle());
    }

    private IEnumerator GetAssetBundle()
    {
        Stopwatch watch = Stopwatch.StartNew();
        // TODO: handle multiple specimen of same organ when server structure known
        foreach (string organ in organs) {
            // TODO load concurrently
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle($"https://hivemodelstorage.blob.core.windows.net/win64assetbundle/{organ}");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                // TODO: figure out structure of server -- are all these healthy? How should they be stored, id'd?
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                GameObject prefab = bundle.LoadAsset($"assets/prefabs/organs_labeled/{organ}_healthy.prefab") as GameObject;
                if (prefab == null) {
                    prefab = bundle.LoadAsset($"assets/prefabs/organs/{organ}_healthy.prefab") as GameObject;
                }

                // TODO: change this when they have other stuff
                string id = $"{organ}_healthy";
                SpecimenData specimenData = new SpecimenData(id, prefab, organ);
                specimens.Add(id, specimenData);
                string region = organToRegion[organ];

                if (!specimensByRegionByOrgan.ContainsKey(region))
                {
                    specimensByRegionByOrgan.Add(region, new Dictionary<string, List<SpecimenData>>());
                }

                if (!specimensByRegionByOrgan[region].ContainsKey(organ))
                {
                    specimensByRegionByOrgan[region].Add(organ, new List<SpecimenData>());
                }

                specimensByRegionByOrgan[region][organ].Add(specimenData);
            }
        }

        _loading = false;
        watch.Stop();

        Debug.Log($"Finished loading specimens.");
        Debug.Log($"Loaded {specimens.Count} total specimens.");
        Debug.Log($"Load time: {watch.Elapsed.TotalSeconds} seconds");
        Debug.Log(DumpCurrentSpecimenStructure());
    }

}
