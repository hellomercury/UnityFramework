using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Tools
{
    public class VisualStudioStarter
    {
        [MenuItem("Framework/Editor/Open Visual Studio %t")]
        public static void VsStarter()
        {
            string path = Application.dataPath.Replace("\\", "/");
            path = path.Replace("/Assets", string.Empty);
            int index = path.LastIndexOf('/');
            string projectName = path.Substring(index, path.Length - index);

            if(path.Contains(":"))
            {
                SystemProcess.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe", path + projectName + ".sln");
            }
            else
            {
                SystemProcess.Start(@"/Applications/Visual Studio.app", path + "/Assembly-CSharp.sln");
                SystemProcess.Start(@"/Applications/Visual Studio.app", path + "/Assembly-CSharp-Editor.sln");
            } 
        }

        //[MenuItem("Framework/Editor/Open Editor Visual Studio %e")]
        //public static void VsEditorStarter()
        //{
        //    string path = Application.dataPath.Replace("\\", "/");
        //    path = path.Replace("/Assets", string.Empty);

        //    if (path.Contains(":"))
        //    {
        //        SystemProcess.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe", path + "/TriPeaks.sln");
        //    }
        //    else
        //    {
        //        SystemProcess.Start(@"/Applications/Visual Studio.app", path + "/Assembly-CSharp-Editor.sln");
        //    }
        //}
    }
}
