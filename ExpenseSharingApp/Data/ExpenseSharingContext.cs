using ExpenseSharingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Group = ExpenseSharingApp.Models.Group;

public class ExpenseSharingContext : DbContext
{
    public ExpenseSharingContext(DbContextOptions<ExpenseSharingContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.TotalExpense).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GroupId });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Groups)
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Group)
                  .WithMany(g => g.Members)
                  .HasForeignKey(e => e.GroupId);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.IsSettled).IsRequired();

            entity.HasOne(e => e.PaidBy)
                  .WithMany()
                  .HasForeignKey(e => e.PaidById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Group)
                  .WithMany()
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });


    }

    public static void Seed(ExpenseSharingContext context)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
                {
                    new User { Id = Guid.NewGuid().ToString(),Name = "Admin", Email = "admin@example.com", Password = "password", Role = "Admin" },
                    new User { Id = Guid.NewGuid().ToString(),Name = "User 01", Email = "user1@example.com", Password = "password", Role = "Normal" },
                    new User { Id = Guid.NewGuid().ToString(),Name = "User 02", Email = "user2@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(),  Name = "User 03", Email = "user3@example.com", Password = "password", Role = "Normal" },
                    new User { Id = Guid.NewGuid().ToString(),Name = "User 04", Email = "user4@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(),  Name = "User 05", Email = "user5@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(),  Name = "User 06", Email = "user6@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(), Name = "User 07", Email = "user7@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(),  Name = "User 08", Email = "user8@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(), Name = "User 09", Email = "user9@example.com", Password = "password", Role = "Normal" },
                    new User {Id = Guid.NewGuid().ToString(),  Name = "User 10", Email = "user10@example.com", Password = "password", Role = "Normal" }
                };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
       
    }
}
