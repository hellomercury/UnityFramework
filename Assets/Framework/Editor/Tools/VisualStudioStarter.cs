using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Tools
{
    public class VisualStudioStarter
    {
        [MenuItem("Framework/Editor/Open Visual Studio")]
        public static void VsStarter()
        {
            string path = Application.dataPath.Replace("\\", "/");
            path = path.Replace("/Assets", string.Empty);

            if(path.Contains(":"))
            {
                SystemProcess.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe", path + "/Assembly-CSharp.sln");
            }
            else
            {
                Debug.LogError(path + "/Assembly-CSharp.sln");
                SystemProcess.Start(@"/Applications/Visual Studio.app", path + "/Assembly-CSharp.sln");
            }
        }
    }
}
