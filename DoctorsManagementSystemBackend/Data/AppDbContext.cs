using DoctorsManagementSystem.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace DoctorsManagementSystem.Data;

public class AppDbContext : DbContext 
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Operation> Operations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var configuartions = new ConfigurationBuilder().AddJsonFile("appSettings.Json").Build();
        var connectionString = configuartions.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }
}