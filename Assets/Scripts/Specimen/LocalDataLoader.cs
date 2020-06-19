using UnityEngine;

public class LocalDataLoader: DataLoader
{
    protected override DataManifest GetManifest()
    {
        TextAsset file = Resources.Load<TextAsset>(manifestPath);
        DataManifest manifest = JsonUtility.FromJson<DataManifest>(file.text);
        return manifest;
    }

}
