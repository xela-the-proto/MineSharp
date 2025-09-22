namespace MineSharpAPI.Routes;

public class Delete
{
    public static void RegisterDeletes(WebApplication app)
    {
        /*
        app.MapDelete("/api/user/rm", async ([FromBody] InquilinoDB inquilino, HttpContext http,
            DatabaseContext db, IDbInquilino dbInquilino) =>
        {
            dbInquilino.RmInquilino(db, inquilino);
            return Task.CompletedTask;
        }).RequireAuthorization();
        */
    }
}