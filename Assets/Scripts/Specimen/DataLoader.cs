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

    protected abstract IEnumerator LoadManifest();
    public string manifestPath;
    public DataManifest manifest;
    public bool _manifestLoaded;

    private List<LabData> _labs;
    private List<SpecimenData> _specimens;
    private List<RegionData> _regions;
    private int _requestsResolved;
    private bool _loaded;

    public void Load()
    {
        StartCoroutine(Loading());
    }

    public bool Loaded()
    {
        return _loaded;
    }

    public List<SpecimenData> GetSpecimens() {
        return _specimens;
    }

    public List<LabData> GetLabs() {
        return _labs;
    }

    public List<RegionData> GetRegions() {
        return _regions;
    }

    private IEnumerator Loading()
    {
        _loaded = false;
        _manifestLoaded = false;
        Stopwatch watch = Stopwatch.StartNew();

        // Wait for the caching system to be ready
        while (!Caching.ready) yield return null;

        // Gets and verifies manifest file
        StartCoroutine(LoadManifest());
        while (!_manifestLoaded) yield return null;
        VerifyManifest(manifest);

        // Tracks the number of requests returned, even if failed
        _requestsResolved = 0;

        _specimens = new List<SpecimenData>();
        foreach (SpecimenRequestData sprd in manifest.specimenData)
        {
            // Load specimen data from request. Must increment requestsResolved when finished, even if failed!
            StartCoroutine(LoadFromData(sprd));
        }


        // Regions and labs are stored directly in the manifests
        _regions = manifest.regions.ToList();
        _labs = manifest.labs.ToList();

        // Wait until all requests are resolved
        while (_requestsResolved < manifest.specimenData.Length) yield return null;
        watch.Stop();

        // Listeners can now check IsLoading() and know that the loader has completed.
        _loaded = true;

        Debug.Log($"Loaded {_specimens.Count} total specimens, {_labs.Count} labs, and {_regions.Count} regions.");
        Debug.Log($"Load time: {watch.Elapsed.TotalSeconds} seconds");
    }

    /**
     * Loads SpecimenData -- including mesh and texture -- from a specimen request
     */
    private IEnumerator LoadFromData(SpecimenRequestData srd)
    {
        using (UnityWebRequest req =
            UnityWebRequestAssetBundle.GetAssetBundle(srd.assetUrl, Convert.ToUInt32(srd.version), 0U))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogWarning($"{req.error} : Could not find bundle for {srd.id} at {srd.assetUrl}");

            } else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                try
                {
                    /*foreach (string file in bundle.GetAllAssetNames ())
                    {
                        Debug.Log(file);
                    }*/

                    // Checks for null material and mesh; if not loadable, will not add the asset.
                    Material mat = bundle.LoadAsset<Material>(srd.matPath);
                    if (mat == null)
                    {
                        Debug.LogWarning($"Could not find material for {srd.id} at path {srd.matPath} in bundle. Please check your bundle structure and try again.");
                        throw new Exception();
                    }

                    Mesh mesh = bundle.LoadAsset<Mesh>(srd.meshPath);
                    if (mesh == null) {
                        Debug.LogWarning($"Could not find mesh for {srd.id} at path {srd.meshPath} in bundle.  Please check your bundle structure and try again.");
                        throw new Exception();
                    }

                    // Asset seems good, add to the specimens list.
                    SpecimenData specimenData = new SpecimenData()
                    {
                        id = srd.id,
                        annotations = srd.annotations.ToList(),
                        material = mat,
                        mesh = mesh, 
                        version = srd.version,
                        organ = srd.organ,
                        scale = srd.scale,
                        name = srd.name
                    };
                    _specimens.Add(specimenData);

                }
                catch (Exception e)
                {
                    Debug.LogWarning($"{e} : Problem with {srd.id}");
                }
            }

            // Must resolve requests in order to trigger loading finished
            _requestsResolved++;
        }

    }

    private bool VerifyManifest(DataManifest manifest)
    {
        if (manifest == null) {
            Debug.LogError("Couldn't load manifest. Make sure you are pointing to a correct, existing online or local resource.");
            return false;
        }

        bool verify = true;

        if (manifest.regions == null)
        {
            Debug.LogError("No region data in loaded manifest. Please add region data.");
            verify = false;
        }

        if (manifest.labs == null) {
            Debug.LogError("No lab data in loaded manifest. Please add lab data if desired.");
            verify = false;
        }

        if (manifest.specimenData == null) {
            Debug.LogError("No specimen data in loaded manifest. Please add specimen data if desired.");
            verify = false;
        }

        return verify;
    }

}
