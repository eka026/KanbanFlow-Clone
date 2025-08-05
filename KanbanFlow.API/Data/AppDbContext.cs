using KanbanFlow.Core;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Column> Columns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity configuration
            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Project entity configuration
            modelBuilder.Entity<Project>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Column entity configuration
            modelBuilder.Entity<Column>()
                .Property(c => c.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Column>()
                .HasOne(c => c.Project)
                .WithMany(p => p.Columns)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Column>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // TaskItem entity configuration
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => new { t.Position, t.ColumnId })
                .IsUnique();

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Column)
                .WithMany(c => c.TaskItems)
                .HasForeignKey(t => t.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}