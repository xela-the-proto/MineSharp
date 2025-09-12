using System.Diagnostics;
using Common.Converters;

namespace Common.Process;

public class ProcessInfoHelper
{
    /// <summary>
    /// Builds the ProcessInfo and return a process
    /// </summary>
    /// <param name="command">The main command (java, python3, ecc.)</param>
    /// <param name="flags">Flags for the command</param>
    /// <param name="values">Flag values</param>
    /// <param name="workDir">When the <c>useShellExecute</c> property is false, gets or sets the working directory for the process to be started.
    /// When <c>useShellExecute</c> is true, gets or sets the directory that contains the process to be started.</param>
    /// <param name="hookIntoStdErr">Make the process <c>StandardError</c> accessible with events</param>
    /// <param name="hookIntoStdOut">Make the process <c>StandardOut</c> accessible with events</param>
    /// <param name="hookIntoStdIn">Make the process <c>StandardInput</c> accessible with events</param>
    /// <param name="useShellExecute">Whether to use the operating system shell to start the process.</param>
    /// <returns></returns>
    public static System.Diagnostics.Process BuildStarterProcess(string command, List<string> flags, List<string> values
        , string workDir, bool hookIntoStdErr = true, bool hookIntoStdOut = true, bool hookIntoStdIn = true, bool useShellExecute = true)
    {
        string args = BuildArguments(ConvertFlagsToJavaFlags.ConvertList(flags, values));
        
        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardError = hookIntoStdErr,
            RedirectStandardOutput = hookIntoStdOut,
            RedirectStandardInput = hookIntoStdIn,
            UseShellExecute = useShellExecute,
            WorkingDirectory = workDir.Substring(
                0, workDir.LastIndexOf(Path.DirectorySeparatorChar) + 1),
        };
        var process = new System.Diagnostics.Process { StartInfo = startInfo };
        return process;
    }

    protected static string BuildArguments(List<string> flags)
    {
        string builtArgs = "";

        foreach (var flag in flags)
        {
            builtArgs += flag + " ";
        }

        var split = builtArgs.Split(".jar");
        builtArgs = split[1] + split[0] + @".jar";
        return builtArgs;
    }
}