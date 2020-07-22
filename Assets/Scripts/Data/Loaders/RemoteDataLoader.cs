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
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogWarning($"Unable to get manifest from ${manifestPath}. Please check your internet connection or contact the administrator.");
            }
            else
            {
                manifest = JsonUtility.FromJson<DataManifest>(req.downloadHandler.text);
                _manifestLoaded = true;
            }
        }
    }
}
