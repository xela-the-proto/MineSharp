namespace Runner;

public class ArgsParser
{
    public static Tuple<List<string>, List<string>> ParseArgs(string[] args)
    {
        //Ex: -v -f
        List<string> typeofArg = new List<string>();
        //Ex: 1.21 , c:\foo\bar
        List<string> typeofArgValue = new List<string>();
        //ORDER IS IMPORTANT
        for (int i = 0; i < args.Length; i++)
        {
            if (i % 2 == 0)
            {
                typeofArg.Add(args[i]);
            }
            else
            {
                if (args[i].Contains(Path.DirectorySeparatorChar))
                {
                    args[i] += Path.DirectorySeparatorChar + "server.jar";
                }
                typeofArgValue.Add(args[i]);
            }
        }
        
        return Tuple.Create(typeofArg,typeofArgValue);
    }
}