using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class MenuTools
    {
        [MenuItem("Framework/Open Files/PersistentData Folder", priority = 30)]
        static void OpenPersistentData()
        {
            SystemProcess.Start(Application.persistentDataPath);
        }

        [MenuItem("Framework/Open Files/Assets Folder", priority = 33)]
        static void OpenAssets()
        {
            SystemProcess.Start(Application.dataPath);
        }

        [MenuItem("Framework/Open Files/StreamingAssets Folder", priority = 35)]
        static void OpenStreamingAssets()
        {
            SystemProcess.Start(Application.streamingAssetsPath);
        }
    }
}