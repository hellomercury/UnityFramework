using System.Diagnostics;

namespace Framework.Editor
{
    public class SystemProcess
    {
        public static void Start(string InCommand, string InArgument = "")
        {
            ProcessStartInfo start = new ProcessStartInfo(InCommand);
            start.Arguments = InArgument;
            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = true;

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
