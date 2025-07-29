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

        public DbSet<Team> Teams { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutResult> WorkoutResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do Team
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalPoints).HasDefaultValue(0);
                
                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuração do Workout
            modelBuilder.Entity<Workout>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Type).IsRequired();
                
                // Índices
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configuração do WorkoutResult
            modelBuilder.Entity<WorkoutResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Result).HasPrecision(10, 2);
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