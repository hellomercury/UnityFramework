using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class GitMenu : MonoBehaviour
{
    [MenuItem("Framework/Git/Open GitBase Here %g")]
    static void OpenGitBase()
    {
        if(Application.dataPath.Contains(":"))
        {
            ProcessCommand(@"C:\Program Files\Git\git-bash.exe");
        }
        else
        {
            ProcessCommand(@"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", "cd " + Application.dataPath.Replace("/Assets", "/"));
        }
        //UnityEngine.Debug.LogError(Application.dataPath);
        //#if UNITY_STANDALONE_OSX
        
        //#else
        //"/user:Administrator \"cmd /K " + command + "\""
        //#endif
    }

    //[MenuItem("Git/Add")]
    static void AddCommand()
    {
        ProcessCommand(@"C:\Program Files\Git\git-bash.exe", "-git add .");
    }

    private static void ProcessCommand(string InCommand, string InArgument = "")
    {
        ProcessStartInfo start = new ProcessStartInfo(InCommand);
        start.Arguments = InArgument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;

        // if (start.UseShellExecute)
        // {
        //     start.RedirectStandardOutput = false;
        //     start.RedirectStandardError = false;
        //     start.RedirectStandardInput = false;
        // }
        // else
        // {
        //     start.RedirectStandardOutput = true;
        //     start.RedirectStandardError = true;
        //     start.RedirectStandardInput = true;
        //     start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
        //     start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        // }

        System.Diagnostics.Process.Start(start);

        //p.WaitForExit();
        //p.Close();
    }
}
