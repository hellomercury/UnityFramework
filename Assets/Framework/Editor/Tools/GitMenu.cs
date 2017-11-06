using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class GitMenu : MonoBehaviour
    {
        [MenuItem("Framework/Editor/Open GitBase Here %g")]
        static void OpenGitBase()
        {
            if (Application.dataPath.Contains(":"))
            {
                SystemProcess.Start(@"C:\Program Files\Git\git-bash.exe");
            }
            else
            {
                SystemProcess.Start(@"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", "cd " + Application.dataPath.Replace("/Assets", "/"));
            }
        }


    }
}

