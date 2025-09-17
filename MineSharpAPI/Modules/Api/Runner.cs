namespace MineSharpAPI.Api;

public class Runner
{
    public void RegisterRunner(RunnerTable runnerDetails, HttpContext context, DatabaseContext db)
    {
        db.Runner.Add(runnerDetails);
        db.SaveChangesAsync();
    }

}