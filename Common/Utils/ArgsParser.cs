using Common.Json;

namespace Runner;

public class ArgsParser
{
    private static readonly Dictionary<string, string> mapping = new()
    {
        { nameof(RunnerBody.ram), "-r" },
        { nameof(RunnerBody.version), "-v" },
        { nameof(RunnerBody.platform), "-p" },
        { nameof(RunnerBody.path), "-f" },
        { nameof(RunnerBody.eulaAccept), "-eula" }
    };

    public static List<string> BuildArgs(RunnerBody body)
    {
        var properties = typeof(RunnerBody).GetProperties();
        var args = new List<string>();

        foreach (var property in properties)
            if (mapping.TryGetValue(property.Name, out var flag))
            {
                var value = property.GetValue(body);

                // Salta se è null o default (0 per int)
                if (value == null) continue;

                if (value is int intVal && intVal == 0) continue;

                args.Add(flag);
                args.Add(value.ToString());
            }

        return args;
    }
}