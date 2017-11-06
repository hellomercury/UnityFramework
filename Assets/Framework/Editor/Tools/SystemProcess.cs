using System.Diagnostics;

namespace Framework.Editor
{
    public class SystemProcess
    {
        public static void Start(string InCommand, string InArgument = "",
            bool InIsCreateNoWindow = true, bool InIsErrorDialog = true, bool InIsUseShellExecute = true)
        {
            ProcessStartInfo start = new ProcessStartInfo(InCommand);
            start.Arguments = InArgument;
            start.CreateNoWindow = InIsCreateNoWindow;
            start.ErrorDialog = InIsErrorDialog;
            start.UseShellExecute = InIsUseShellExecute;

            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = System.Text.Encoding.UTF8;
                start.StandardErrorEncoding = System.Text.Encoding.UTF8;
            }

            Process.Start(start);
        }
    }
}
