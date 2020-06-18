using System;
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
    public Dictionary<string, SpecimenData> specimens;
    public Dictionary<string, Dictionary<string, List<SpecimenData>>> specimensByRegionByOrgan;

    public string[] regions = {"Head", "Thorax", "Abdomen"};
    public string[] organs = {"brain", "heart", "kidney", "liver", /*"right lung",*/ "skull"};

    public Dictionary<string, string> organToRegion = new Dictionary<string, string>
    {
        {"brain", "Head"},
        {"heart", "Thorax"},
        {"kidney", "Abdomen"},
        {"liver", "Abdomen"},
        {"skull", "Head"}
    };


    // TODO: This should be received from server when we have structured data
    public List<SpecimenRequestData> requestData = new List<SpecimenRequestData>
    {
        new SpecimenRequestData("brain01", $"assets/prefabs/organs_labeled/brain_healthy.prefab", "brain",
            "https://hivemodelstorage.blob.core.windows.net/win64assetbundle/brain", 0),
        new SpecimenRequestData("heart01", $"assets/prefabs/organs/heart_healthy.prefab", "heart",
            "https://hivemodelstorage.blob.core.windows.net/win64assetbundle/heart", 0),
        new SpecimenRequestData("kidney01", $"assets/prefabs/organs_labeled/kidney_healthy.prefab", "kidney",
            "https://hivemodelstorage.blob.core.windows.net/win64assetbundle/kidney", 0),
        new SpecimenRequestData("liver01", $"assets/prefabs/organs_labeled/liver_healthy.prefab", "liver",
            "https://hivemodelstorage.blob.core.windows.net/win64assetbundle/liver", 0),
        new SpecimenRequestData("skull01", $"assets/prefabs/organs_labeled/skull_healthy.prefab", "skull",
            "https://hivemodelstorage.blob.core.windows.net/win64assetbundle/skull", 0)
    };

    private int _requestsResolved;
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
        return specimens.Values.Where(spd => !filteredOutIds.Contains(spd.Id)).ToList();
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

    private void Start()
    {
        StartCoroutine(GetAssetBundles());
    }

    private IEnumerator GetAssetBundles()
    {
        Stopwatch watch = Stopwatch.StartNew();

        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        _requestsResolved = 0;
        foreach (SpecimenRequestData srd in requestData)
        {
            StartCoroutine(LoadFromData(srd));
        }

        while (_requestsResolved < requestData.Count) yield return null;
        _loading = false;
        watch.Stop();

        Debug.Log($"Finished loading specimens.");
        Debug.Log($"Loaded {specimens.Count} total specimens.");
        Debug.Log($"Load time: {watch.Elapsed.TotalSeconds} seconds");
        Debug.Log(DumpCurrentSpecimenStructure());
    }

    private IEnumerator LoadFromData(SpecimenRequestData srd)
    {

        specimensByRegionByOrgan = new Dictionary<string, Dictionary<string, List<SpecimenData>>>();
        specimens = new Dictionary<string, SpecimenData>();

        //UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(srd.assetUrl);
        using (UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(srd.assetUrl, Convert.ToUInt32(srd.version), 0U))
        {
            Debug.Log(Caching.currentCacheForWriting.path);
            Stopwatch watch = Stopwatch.StartNew();
            Debug.Log($"started load {srd.id}");

            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError) {
                Debug.Log(req.error);
            } else {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                try {
                    GameObject prefab = bundle.LoadAsset(srd.path) as GameObject;

                    SpecimenData specimenData = new SpecimenData(srd.id, prefab, srd.organ);
                    specimens.Add(srd.id, specimenData);
                    string region = organToRegion[srd.organ];

                    if (!specimensByRegionByOrgan.ContainsKey(region)) {
                        specimensByRegionByOrgan.Add(region, new Dictionary<string, List<SpecimenData>>());
                    }

                    if (!specimensByRegionByOrgan[region].ContainsKey(srd.organ)) {
                        specimensByRegionByOrgan[region].Add(srd.organ, new List<SpecimenData>());
                    }

                    specimensByRegionByOrgan[region][srd.organ].Add(specimenData);
                } catch (Exception e) {
                    Debug.LogWarning(e);
                }
            }

            watch.Stop();
            _requestsResolved++;
        }

       /* if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            
           
        }
        Debug.Log($"finished load {srd.id}, took {watch.Elapsed.TotalSeconds}");*/

    }
}