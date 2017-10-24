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
            public string AssetFileName;
            public string LoaclFileName;
            public bool IsNeedCheck;
            public string Md5;
            public bool IsUnzip;
            public float FileSize;

            public VersionTemp()
            {

            }

            public Version GetVersion()
            {
                return new Version(AssetFileName, LoaclFileName, Md5, IsNeedCheck, IsUnzip, FileSize);
            }
        }
        private static List<VersionTemp> versions;
        private static string currentVersion;

        [MenuItem("Framework/Create Version Window")]
        private static void Init()
        {
            DirectoryInfo info = new DirectoryInfo(Application.streamingAssetsPath);

            List<FileInfo> fileInfos = Utility.GetAllFileInfoFromTheDirectory(info);
            string streamPath = Application.streamingAssetsPath.Replace("\\", "/") + "/";
            versions = new List<VersionTemp>();
            int count = fileInfos.Count;
            for (int i = 0; i < count; i++)
            {
                if (fileInfos[i].Extension.Equals(".meta")) continue;
                
                VersionTemp version = new VersionTemp();
                version.IsUseful = true;
                version.AssetFileName = fileInfos[i].FullName.Replace("\\", "/").Replace(streamPath, "");
                version.LoaclFileName = fileInfos[i].Name;
                version.IsNeedCheck = true;
                version.Md5 = Utility.GetFileMD5(fileInfos[i].FullName);
                version.FileSize = fileInfos[i].Length;
                version.IsUnzip = false;
                versions.Add(version);
            }

            VersionList versionList = AssetDatabase.LoadAssetAtPath<VersionList>("Assets/Resources/VersionList/Version.asset");
            currentVersion = null == versionList ? "1.0.0" : versionList.CurrentVersion;

            CreateVersionList window = EditorWindow.CreateInstance<CreateVersionList>();
            window.maxSize = new Vector2(440, 640);
            window.minSize = new Vector2(440, 640);
            window.Show();
        }

        private Vector2 scrollViewPos = Vector2.zero;
        void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Version Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            currentVersion = EditorGUILayout.TextField("当前版本号：", currentVersion);

            if(GUILayout.Button("Create"))
            {
                List<Version> allFiles = new List<Version>();
                List<Version> needCheckFiles = new List<Version>();
                for (int i = 0; i < versions.Count; i++)
                {
                    if(versions[i].IsUseful)
                    {
                        Version version = versions[i].GetVersion();
                        allFiles.Add(version);

                        if(versions[i].IsNeedCheck) needCheckFiles.Add(version);
                    }
                }

                VersionList versionList = ScriptableObject.CreateInstance<VersionList>();
                versionList.CurrentVersion = currentVersion;
                versionList.Files = allFiles.ToArray();
                versionList.FilesNeedCheck = needCheckFiles.ToArray();

                AssetDatabase.CreateAsset(versionList, "Assets/Resources/VersionList/Version.asset");
                AssetDatabase.Refresh();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Resources/VersionList/Version.asset");
                Close();
            }
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
            if (null != versions)
            {
                int count = versions.Count;
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginVertical("box");
                    versions[i].IsUseful = EditorGUILayout.BeginToggleGroup(versions[i].AssetFileName, versions[i].IsUseful);
                    versions[i].AssetFileName = EditorGUILayout.TextField("项目内资源路径：", versions[i].AssetFileName);
                    versions[i].LoaclFileName = EditorGUILayout.TextField("本地存储资源名：", versions[i].LoaclFileName);
                    versions[i].IsNeedCheck = EditorGUILayout.Toggle("是否需要每次登陆校验：", versions[i].IsNeedCheck);
                    EditorGUILayout.LabelField("资源唯一标示(MD5)：", versions[i].Md5);
                    versions[i].IsUnzip = EditorGUILayout.Toggle("是否需要进行解压：", versions[i].IsUnzip);
                    EditorGUILayout.LabelField("资源文件大小：", versions[i].FileSize / 1024 + "KB");
                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
