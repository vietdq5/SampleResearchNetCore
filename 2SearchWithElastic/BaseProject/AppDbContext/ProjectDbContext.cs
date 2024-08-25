using BaseProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.DataContext;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<SubUnit> SubUnits { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubUnit>().ToTable("SubUnits").HasKey(x => x.Id);

        modelBuilder.Entity<Employee>().ToTable("Employees")
        .HasOne<SubUnit>(s => s.SubUnits)
        .WithMany(g => g.Employees)
        .HasForeignKey(s => s.SubUnitId);

        modelBuilder.Entity<Employee>()
        .HasOne(s => s.SupperEmployee)
        .WithMany(g => g.Employees)
        .HasForeignKey(s => s.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa cha nếu có con;

        modelBuilder.Entity<Category>()
          .HasOne(c => c.Parent)
          .WithMany(c => c.Children)
          .HasForeignKey(c => c.ParentId)
          .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa cha nếu có con
    }

    public override int SaveChanges()
    {
        var dateNow = DateTime.UtcNow;
        var errorList = new List<ValidationResult>();

        var entries = ChangeTracker.Entries()
            .Where(p => p.State == EntityState.Added ||
                        p.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
            {
                if (entity is BaseEntities itemBase)
                {
                    itemBase.CreateDate = itemBase.UpdateDate = dateNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entity is BaseEntities itemBase)
                {
                    itemBase.UpdateDate = dateNow;
                }
            }

            Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
        }

        if (errorList.Count != 0)
        {
            throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var dateNow = DateTime.UtcNow;
        var errorList = new List<ValidationResult>();

        var entries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Modified).ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
            {
                if (entity is BaseEntities itemBase)
                {
                    itemBase.CreateDate = itemBase.UpdateDate = dateNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entity is BaseEntities itemBase)
                {
                    itemBase.UpdateDate = dateNow;
                }
            }

            Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
        }

        if (errorList.Count != 0)
        {
            throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}