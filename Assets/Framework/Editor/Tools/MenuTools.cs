using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuTools
{
    [MenuItem("Framework/Open Files/PersistentData Folder", priority = 30)]
    static void OpenPersistentData()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Framework/Open Files/Assets Folder", priority = 33)]
    static void OpenAssets()
    {
        System.Diagnostics.Process.Start(Application.dataPath);
    }

    [MenuItem("Framework/Open Files/StreamingAssets Folder", priority = 35)]
    static void OpenStreamingAssets()
    {
        System.Diagnostics.Process.Start(Application.streamingAssetsPath);
    }
}
