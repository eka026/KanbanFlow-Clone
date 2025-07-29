using KanbanFlow.Core;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Column> Columns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => new { t.Position, t.ColumnId })
                .IsUnique();
        }
    }
}