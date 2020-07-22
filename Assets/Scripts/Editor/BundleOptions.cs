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

    [MenuItem("Custom Utilities/Build WebGL")]
    static void PerformBuildWebGl() {
        BuildPipeline.BuildAssetBundles("../AssetBundles/", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
    }

    [MenuItem("Custom Utilities/Build MacOs")]
    static void PerformBuildOSX() {
        BuildPipeline.BuildAssetBundles("../AssetBundles/", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
    }
}
