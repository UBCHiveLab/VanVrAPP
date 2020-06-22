using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDataLoader: DataLoader
{
    protected override IEnumerator LoadManifest()
    {
        TextAsset file = Resources.Load<TextAsset>(manifestPath);
        manifest = JsonUtility.FromJson<DataManifest>(file.text);
        _manifestLoaded = true;
        yield break;
    }

}
