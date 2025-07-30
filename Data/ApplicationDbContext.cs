using CrossfitLeaderboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutResult> WorkoutResults { get; set; }
        public DbSet<WorkoutCategory> WorkoutCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired(false);
                
                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuração do Team
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalPoints).HasDefaultValue(0);
                entity.Property(e => e.CategoryId).IsRequired(false);

                // Relacionamento com Category
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Teams)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Índices
                entity.HasIndex(e => new { e.Name, e.CategoryId }).IsUnique();
            });

            // Configuração do Workout
            modelBuilder.Entity<Workout>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired(false).HasMaxLength(500);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Type).IsRequired();
                
                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuração do WorkoutCategory (many-to-many)
            modelBuilder.Entity<WorkoutCategory>(entity =>
            {
                entity.HasKey(e => new { e.WorkoutId, e.CategoryId });

                entity.HasOne(e => e.Workout)
                    .WithMany(w => w.WorkoutCategories)
                    .HasForeignKey(e => e.WorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.WorkoutCategories)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração do WorkoutResult
            modelBuilder.Entity<WorkoutResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                // SQLite não suporta HasPrecision, então removemos
                entity.Property(e => e.Result);
                entity.Property(e => e.Position).HasDefaultValue(0);
                entity.Property(e => e.Points).HasDefaultValue(0);

                // Relacionamentos
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.WorkoutResults)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Workout)
                    .WithMany()
                    .HasForeignKey(e => e.WorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Índices compostos
                entity.HasIndex(e => new { e.TeamId, e.WorkoutId }).IsUnique();
            });
        }
    }
} 