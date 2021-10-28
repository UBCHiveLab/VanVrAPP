using UnityEditor;
using UnityEngine;

public static class ClearCache
{
    [MenuItem("Custom Utilities/Clear Asset Cache")]
    static void PerformClear()
    {
        Debug.Log("Successfully Clear Cache: "+Caching.ClearCache());
    }

}
