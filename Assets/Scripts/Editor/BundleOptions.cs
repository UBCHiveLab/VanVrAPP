using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BundleOptions : MonoBehaviour
{
    [MenuItem("Custom Utilities/Build StandaloneWindows")]
    static void PerformBuild() {
        BuildPipeline.BuildAssetBundles("../AssetBundles/", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }
}
