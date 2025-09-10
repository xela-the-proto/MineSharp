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
        builder.Entity<UserDB>(entity => {
            entity.HasIndex(e => e.Email).IsUnique();
        });
        builder.Entity<UserDB>().HasData();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }
    
    //Registro i Dbset così EF sa cosa mettere nel db
    public DbSet<UserDB> User { get; set; }
    public DbSet<RunnerDB> Runner { get; set; }
}

/*
 * -------------------------------------------
 *  TABLES
 * -------------------------------------------
 */
public record UserDB
{
    [StringLength(36)]
    [Required]
    public string UserId { get; set; }
    [StringLength(40)]
    [Required]
    [Key] 
    public string Email { get; set; }
    [StringLength(97)]
    [Required] 
    public string PasswordHash { get; set; }
}


public record RunnerDB
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
}