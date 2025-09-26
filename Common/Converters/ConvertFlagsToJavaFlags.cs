namespace Common.Converters;

public class ConvertFlagsToJavaFlags
{
    public static List<string> ConvertList(List<string> flags)
    {
        List<string> javaFlags = new List<string>();
        //We automatically skip by 1
        int i = 0;
        foreach (var flag in flags)
        {
            switch (flag)
            {
                case "-f":
                    javaFlags.Add($"-jar {flags[i + 1]}/server.jar");
                    break;
                case "-r":
                    javaFlags.Add($"-Xmx{flags[i + 1]}m");
                    javaFlags.Add($"-Xms{flags[i + 1]}m");
                    break;
            }
            i++;
        }
        //Swap the file and ram the other way around
        var temp = javaFlags[0];
        javaFlags[0] = javaFlags[1];
        javaFlags[1] = javaFlags[2];
        javaFlags[2] = temp;
        
        return javaFlags;
    }
}