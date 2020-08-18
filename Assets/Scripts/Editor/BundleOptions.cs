using UnityEngine;
using UnityEditor;

public class BundleOptions : MonoBehaviour
{
    [MenuItem("Custom Utilities/Build StandaloneWindows")]
    static void PerformBuild() {
        BuildPipeline.BuildAssetBundles(Application.dataPath + "/Models/Specimens/AssetBundles/win", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Custom Utilities/Build WebGL")]
    static void PerformBuildWebGl() {
        BuildPipeline.BuildAssetBundles(Application.dataPath + "/Models/Specimens/AssetBundles/webgl", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
    }

    [MenuItem("Custom Utilities/Build MacOs")]
    static void PerformBuildOSX() {
        BuildPipeline.BuildAssetBundles(Application.dataPath + "/Models/Specimens/AssetBundles/osx", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
    }
}
