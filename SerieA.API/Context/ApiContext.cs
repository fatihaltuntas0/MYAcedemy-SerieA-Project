using Microsoft.EntityFrameworkCore;
using SerieA.API.Entities;

namespace SerieA.API.Context
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchGoal> MatchGoals { get; set; }
        public DbSet<MatchCard> MatchCards { get; set; }
        public DbSet<Substitution> Substitutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Match and Team (HomeTeam) Relationship
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Match and Team(AwayTeam) Relationship
            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchGoal>()
                .HasOne(g => g.Match)
                .WithMany(g => g.MatchGoals)
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade); // If the game is canceled, the goals should be canceled too

            modelBuilder.Entity<MatchGoal>()
                .HasOne(g => g.Team)
                .WithMany(g => g.MatchGoals)
                .HasForeignKey(g => g.TeamId)
                .OnDelete(DeleteBehavior.Restrict); // If the team is deleted, its goals should not be deleted. We used “Restrict” to break the loop.

            // MatchCard Relationships
            modelBuilder.Entity<MatchCard>()
                .HasOne(c => c.Match)
                .WithMany(m => m.MatchCards)
                .HasForeignKey(c => c.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchCard>()
                .HasOne(c => c.Team)
                .WithMany()
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Substitution Relationships
            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.Match)
                .WithMany(m => m.Substitutions)
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
