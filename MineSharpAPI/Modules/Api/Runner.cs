using MineSharpAPI.Api;

namespace MineSharpAPI.Modules.Api;

public class Runner
{
    public void RegisterRunner(Runners runnerDetails, HttpContext context, DatabaseContext db)
    {
        db.Runner.Add(runnerDetails);
        db.SaveChangesAsync();
    }

}