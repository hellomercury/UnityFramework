using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefs : MonoBehaviour
{
    [MenuItem("Framework/Clear Local Data/PlayerPrefs")]
    private static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

<<<<<<< HEAD
    //[MenuItem("Framework/Clear Local Data/EditorPrefs")]
    //private static void ClearEditorAllPlayerPrefs()
    //{
    //    Debug.LogError(EditorPrefs.GetString(EditorConfig.EditorPrefsKeys, ""));
    //    string[] keys = EditorPrefs.GetString(EditorConfig.EditorPrefsKeys, "").Split(',');
    //    int length = keys.Length;
    //    for (int i = 0; i < length; i++)
    //    {
    //        EditorPrefs.DeleteKey(keys[i]);
    //    }
    //    EditorPrefs.SetString(EditorConfig.EditorPrefsKeys, "");
    //}
=======
    [MenuItem("Framework/Clear Local Data/EditorPrefs")]
    private static void ClearEditorAllPlayerPrefs()
    {
        Debug.LogError(EditorPrefs.GetString(EditorConfig.EditorPrefsKeys, ""));
        string[] keys = EditorPrefs.GetString(EditorConfig.EditorPrefsKeys, "").Split(',');
        int length = keys.Length;
        for (int i = 0; i < length; i++)
        {
            EditorPrefs.DeleteKey(keys[i]);
        }
        EditorPrefs.SetString(EditorConfig.EditorPrefsKeys, "");
    }
>>>>>>> fa0bab98ab1569fdd29a2c569277111a99ed536c
}
