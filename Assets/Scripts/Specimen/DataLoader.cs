using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public abstract class DataLoader: MonoBehaviour
{

    protected abstract DataManifest GetManifest();

    public string manifestPath;

    private List<LabData> _labs;
    private List<SpecimenData> _specimens = new List<SpecimenData>();
    private List<RegionData> _regions;
    private int _requestsResolved;
    private bool _loaded = false;

    public void Load()
    {
        StartCoroutine(Loading());
    }

    public bool Loaded()
    {
        return _loaded;
    }

    private IEnumerator Loading()
    {
        Stopwatch watch = Stopwatch.StartNew();

        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        DataManifest manifest = GetManifest();
        _requestsResolved = 0;

        foreach (SpecimenRequestData sprd in manifest.specimenData)
        {
            // load specimen data
            StartCoroutine(LoadFromData(sprd));
            
        }

        _regions = manifest.regions.ToList();
        _labs = manifest.labs.ToList();
        while (_requestsResolved < manifest.specimenData.Length) yield return null;
        watch.Stop();
        _loaded = true;


        Debug.Log($"Finished loading specimens.");
        Debug.Log($"Loaded {_specimens.Count} total specimens.");
        Debug.Log($"Load time: {watch.Elapsed.TotalSeconds} seconds");
    }

    public List<SpecimenData> GetSpecimens()
    {
        return _specimens;
    }

    public List<LabData> GetLabs()
    {
        return _labs;
    }

    public List<RegionData> GetRegions()
    {
        return _regions;
    }

    private IEnumerator LoadFromData(SpecimenRequestData srd)
    {
        //UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(srd.assetUrl);
        using (UnityWebRequest req =
            UnityWebRequestAssetBundle.GetAssetBundle(srd.assetUrl, Convert.ToUInt32(srd.version), 0U))
        {
            Debug.Log(Caching.currentCacheForWriting.path);
            Stopwatch watch = Stopwatch.StartNew();
            Debug.Log($"started load {srd.id}");

            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                try
                {
                    /*foreach (string a in bundle.GetAllAssetNames ())
                    {
                        Debug.Log(a);
                    }*/

                    SpecimenData specimenData = new SpecimenData()
                    {
                        id = srd.id,
                        annotations = srd.annotations.ToList(),
                        material = bundle.LoadAsset<Material>(srd.matPath),
                        mesh = bundle.LoadAsset<Mesh>(srd.meshPath),
                        version = srd.version,
                        organ = srd.organ,
                        scale = srd.scale,
                        name = srd.name
                    };
                    _specimens.Add(specimenData);

                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            watch.Stop();
            _requestsResolved++;
        }

    }

}
