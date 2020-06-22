using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteDataLoader : DataLoader
{

    protected override IEnumerator LoadManifest()
    {
        using (UnityWebRequest req =
            UnityWebRequest.Get(manifestPath))
        {
            Debug.Log(req);
            yield return req.SendWebRequest();
            manifest = JsonUtility.FromJson<DataManifest>(req.downloadHandler.text);
            _manifestLoaded = true;
        }
    }
}
