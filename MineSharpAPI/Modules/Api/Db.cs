using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;

namespace MineSharpAPI.Modules.Api;

/*
 * -------------------------------------------
 *  ORM PER INQUILINO
 * -------------------------------------------
 */
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }


    public DbSet<User> User { get; set; }
    public DbSet<Runners> Runner { get; set; }
    public DbSet<APIKeys> ApiKeys { get; set; }
    public DbSet<Server> Server { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        /*
        //Per evitare di dover controllare l'unicità della mail ad ogny query
        builder.Entity<User>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
        builder.Entity<User>().HasData();
        */
    }

    public void OnDbSavingChanges(object? sender, SavingChangesEventArgs args)
    {
        Log.Warning("Db Syncyng");
    }
}


public class DbContextFactory : IDesignTimeDbContextFactory<DbContext>
{
    public DbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        return new DbContext(optionsBuilder.Options);
    }
}


public record User
{
    [StringLength(36)] [Required] [Key] public string Id { get; set; }

    [StringLength(40)] [Required] public string Email { get; set; }

    [StringLength(97)] [Required] public string PasswordHash { get; set; }
}

public record Runners
{
    [StringLength(36)] [Required] [Key] public string Id { get; set; }

    [StringLength(15)] [Required] public string PublicIp { get; set; }
    
    [Required] public List<int> OpenPorts { get; set; }
    
    [Required] public List<string> ServerHardware { get; set; }
}

public record APIKeys
{
    [StringLength(50)] [Required] [Key] public string keyName { get; set; }

    [Required] public string Key { get; set; }

    [Required] [ForeignKey("User")] public string OwnerID { get; set; }
}

public record Server
{
    [StringLength(50)] [Required] [Key] public string id { get; set; }

    [Required] [ForeignKey("Runners")] public string parentRunner { get; set; }

    [Required] public string name { get; set; }

    [Required] public ServerStatus status { get; set; }

    [Required] public float usage { get; set; }

    [Required] public int wsPort { get; set; }

    [Required] public bool IsEulaAccepted { get; set; }
    
    [Required] public int ProcessId { get; set; }
}