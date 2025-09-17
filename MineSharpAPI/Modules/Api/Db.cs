using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DbUpdateException = System.Data.Entity.Infrastructure.DbUpdateException;

namespace MineSharpAPI.Api;
/*
 * -------------------------------------------
 *  ORM PER INQUILINO
 * -------------------------------------------
 */
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //Per evitare di dover controllare l'unicità della mail ad ogny query
        builder.Entity<UserTable>(entity => {
            entity.HasIndex(e => e.Email).IsUnique();
        });
        builder.Entity<UserTable>().HasData();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }
    
    //Registro i Dbset così EF sa cosa mettere nel db
    public DbSet<UserTable> User { get; set; }
    public DbSet<RunnerTable> Runner { get; set; }
    public DbSet<APIKeysTable> ApiKeys { get; set; }
    
}

/*
 * -------------------------------------------
 *  TABLES
 * -------------------------------------------
 */
public record UserTable
{
    [StringLength(36)]
    [Required]
    [Key]
    public string Id { get; set; }
    [StringLength(40)]
    [Required]
    public string Email { get; set; }
    [StringLength(97)]
    [Required] 
    public string PasswordHash { get; set; }
    public ICollection<APIKeysTable>  APIKeys { get; set; }
    public ICollection<RunnerTable>  Runners { get; set; }
}


public record RunnerTable
{
    [StringLength(36)]
    [Required]
    [Key]
    public string RunnerId { get; set; }
    [StringLength(15)]
    [Required]
    public string RunnerPublicIp { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    [ForeignKey("UserTable")]
    public string OwnerID { get; set; }

}

public record APIKeysTable
{
    [StringLength(50)]  
    [Required]
    [Key]
    public string keyName { get; set; }
    [Required]
    public string Key { get; set; }
    [Required]
    [ForeignKey("UserTable")]
    public string OwnerID { get; set; }
}