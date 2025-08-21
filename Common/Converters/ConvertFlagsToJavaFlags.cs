namespace Common.Converters;

public class ConvertFlagsToJavaFlags
{
    public static List<string> ConvertList(List<string> flags, List<string> values)
    {
        List<string> javaFlags = new List<string>();
        //We automatically skip by 1
        int i = 0;
        foreach (var flag in flags)
        {
            switch (flag)
            {
                case "-f":
                    javaFlags.Add($"-jar {values[i]}");
                    break;
                case "-r":
                    javaFlags.Add($"-Xmx{values[i]}M");
                    javaFlags.Add($"-Xms{values[i]}M");
                    break;
            }
            i++;
        }
        
        return javaFlags;
    }

    public static List<string> ConvertArray(string[] flags, string[] values)
    {
        return ConvertList(flags.ToList(),values.ToList());
    }
}