namespace MineSharpAPI.Api;

public class Runner
{
    public void RegisterRunner(RunnerDB runnerDetails, HttpContext context, DatabaseContext db)
    {
        db.Runner.Add(runnerDetails);
        db.SaveChangesAsync();
    }

}