using Common.Process;

namespace Runner.RunnerManager;

public class ServerRunner
{
    public void startServerProcess(List<string> flags ,List<string> values ,string workDir)
    {
        var process = ProcessInfoHelper.BuildStarterProcess("java", flags, values, workDir,true,true,true,false);
        process.OutputDataReceived += (sender, args) =>
        {
            Console.WriteLine(args.Data);
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            Console.WriteLine(args.Data);
        };
        Console.Clear();
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        while (process.HasExited) 
        { 
            var input = Console.ReadLine();
            if (input != null && input.ToLower() != "stop")
            { 
                process.StandardInput.WriteLine(input);
            }
            else
            {
                process.StandardInput.WriteLine("stop");
                break;
            }
        }
    }
}