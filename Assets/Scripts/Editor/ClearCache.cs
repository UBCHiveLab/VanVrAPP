﻿using UnityEditor;
using UnityEngine;

public static class ClearCache
{
    [MenuItem("Custom Utilities/Clear Asset Cache")]
    static void PerformClear()
    {
        Caching.ClearCache();
    }

}
