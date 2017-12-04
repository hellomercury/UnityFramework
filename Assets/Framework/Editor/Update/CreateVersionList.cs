using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework.Tools;
using Framework.Update;

namespace Framework.Editor
{
    public class CreateVersionList : EditorWindow
    {
        class VersionTemp
        {
            public bool IsUseful;
            public string AssetName;
            public string LocalName;
            public bool IsNeedCheck;
            public string Md5;
            public bool IsUnzip;
            public float FileSize;

            public VersionTemp(bool InIsUseful, string InAssetName, string InLocalName, bool InIsNeedCheck,
                               bool InIsUnzip, string InMd5, float InFileSize)
            {
                IsUseful = InIsUseful;
                AssetName = InAssetName;
                LocalName = InLocalName;
                IsNeedCheck = InIsNeedCheck;
                IsUnzip = InIsUnzip;
                Md5 = InMd5;
                FileSize = InFileSize;
            }

            public Version GetVersion()
            {
                return new Version(AssetName, LocalName, Md5, IsNeedCheck, IsUnzip, FileSize);
            }
        }

        private static List<VersionTemp> versions;
        private static string currentVersion;

        [MenuItem("Framework/Create Version Window")]
        private static void Init()
        {
            CreateVersionList window = CreateInstance<CreateVersionList>();
            window.titleContent = new GUIContent("Version", "Create a list of files under the StreamingAssets directory.");
            window.maxSize = new Vector2(555, 600);
            window.minSize = new Vector2(555, 600);
            window.ShowUtility();
        }


        private string streamingAssetsPath, streamingAssetsPathPrefsKey;
        private string resourcesPath, resourcesPathPrefsKey;

        private void OnEnable()
        {
            streamingAssetsPathPrefsKey = "StreamingAssetsPathPrefsKey";
            if (Directory.Exists(Application.streamingAssetsPath)) streamingAssetsPath = "Assets/StreamingAssets/";
            else streamingAssetsPath = EditorPrefs.GetString(streamingAssetsPathPrefsKey, string.Empty);
            EditorPrefs.SetString(streamingAssetsPathPrefsKey, streamingAssetsPath);

            resourcesPathPrefsKey = "ResourcesPathPrefsKey";
            if (Directory.Exists(Application.dataPath + "/Resources")) resourcesPath = "Assets/Resources/";
            else if (Directory.Exists(Application.dataPath + "/resources")) resourcesPath = "Assets/resources/";
            else resourcesPath = EditorPrefs.GetString(resourcesPathPrefsKey, string.Empty);
            EditorPrefs.SetString(resourcesPathPrefsKey, resourcesPath);

            VersionList versionList = AssetDatabase.LoadAssetAtPath<VersionList>(resourcesPath + "Version.asset");
            currentVersion = null == versionList ? "1.0.0" : versionList.CurrentVersion;
        }

        private Vector2 scrollViewPos = Vector2.zero;
        void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Version Creator", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        streamingAssetsPath = EditorGUILayout.TextField("StreamingAssets Path", streamingAssetsPath, GUILayout.Width(440));
                        if (GUILayout.Button("Select", GUILayout.Width(88)))
                        {
                            streamingAssetsPath = EditorUtility.OpenFolderPanel("Choose a database storage location.", Application.dataPath, "StreamingAssets");
                            streamingAssetsPath = "Assets" + streamingAssetsPath.Replace(Application.dataPath, string.Empty).Replace("\\", "/") + "/";
                            EditorPrefs.SetString(streamingAssetsPathPrefsKey, streamingAssetsPath);
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        resourcesPath = EditorGUILayout.TextField("Resouces Path", resourcesPath, GUILayout.Width(440));
                        if (GUILayout.Button("Select", GUILayout.Width(88)))
                        {
                            resourcesPath = EditorUtility.OpenFolderPanel("Choose the resources.", Application.dataPath, "Resources");
                            resourcesPath = "Assets" + resourcesPath.Replace(Application.dataPath, string.Empty).Replace("\\", "/") + "/";
                            EditorPrefs.SetString(resourcesPathPrefsKey, resourcesPath);
                            VersionList versionList = AssetDatabase.LoadAssetAtPath<VersionList>(resourcesPath + "Version.asset");
                            currentVersion = null == versionList ? "1.0.0" : versionList.CurrentVersion;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    currentVersion = EditorGUILayout.TextField("Current version", currentVersion);

                    if (GUILayout.Button("Preview"))
                    {
                        DirectoryInfo info = new DirectoryInfo(streamingAssetsPath);

                        if (!info.Exists)
                        {
                            streamingAssetsPath = EditorUtility.OpenFolderPanel("Choose a database storage location.", Application.dataPath, "StreamingAssets");
                            streamingAssetsPath = "Assets" + streamingAssetsPath.Replace(Application.dataPath, string.Empty).Replace("\\", "/") + "/";
                            EditorPrefs.SetString(streamingAssetsPathPrefsKey, streamingAssetsPath);
                            info = new DirectoryInfo(streamingAssetsPath);
                        }
                        List<FileInfo> fileInfos = Utility.GetAllFileInfoFromTheDirectory(info);
                        versions = new List<VersionTemp>();
                        int count = fileInfos.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (fileInfos[i].Extension.Equals(".meta")) continue;

                            versions.Add(new VersionTemp(true,
                                                         fileInfos[i].FullName.Replace("\\", "/").Replace(Application.dataPath + streamingAssetsPath.Remove(0, 6), ""),
                                                         fileInfos[i].Name,
                                                         true,
                                                         false,
                                                         Utility.GetFileMD5(fileInfos[i].FullName),
                                                         fileInfos[i].Length));
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                {

                    scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
                    if (null != versions)
                    {
                        int count = versions.Count;
                        for (int i = 0; i < count; i++)
                        {
                            EditorGUILayout.BeginVertical("box");
                            versions[i].IsUseful = EditorGUILayout.BeginToggleGroup(versions[i].AssetName, versions[i].IsUseful);
                            EditorGUILayout.LabelField("Name in project", versions[i].AssetName);
                            versions[i].LocalName = EditorGUILayout.TextField("Name in local", versions[i].LocalName);
                            versions[i].IsNeedCheck = EditorGUILayout.Toggle("Need to check", versions[i].IsNeedCheck);
                            versions[i].IsUnzip = EditorGUILayout.Toggle("Need to extract", versions[i].IsUnzip);
                            EditorGUILayout.LabelField("Resource MD5", versions[i].Md5);
                            EditorGUILayout.LabelField("Resource size", versions[i].FileSize / 1024 + "KB");
                            EditorGUILayout.EndToggleGroup();
                            EditorGUILayout.EndVertical();
                        }

                        if (GUILayout.Button("Create"))
                        {
                            List<Version> allFiles = new List<Version>();
                            List<Version> needCheckFiles = new List<Version>();
                            for (int i = 0; i < versions.Count; i++)
                            {
                                if (versions[i].IsUseful)
                                {
                                    Version version = versions[i].GetVersion();
                                    allFiles.Add(version);

                                    if (versions[i].IsNeedCheck) needCheckFiles.Add(version);
                                }
                            }

                            VersionList versionList = CreateInstance<VersionList>();
                            versionList.CurrentVersion = currentVersion;
                            versionList.Files = allFiles.ToArray();
                            versionList.FilesNeedCheck = needCheckFiles.ToArray();

                            string savePath = resourcesPath + "Version.asset";
                            AssetDatabase.CreateAsset(versionList, savePath);
                            AssetDatabase.Refresh();
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(savePath);
                            Close();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
