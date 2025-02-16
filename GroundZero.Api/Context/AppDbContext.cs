using GroundZero.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroundZero.Api.Context;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Decision> Decisions { get; set; }
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Judge> Judges { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<ParticipantReview> ParticipantReviews { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Judge>()
            .HasOne(j => j.NextTeam)
            .WithOne(t => t.NextJudge)
            .HasForeignKey<Judge>(j => j.NextTeamId);

        builder.Entity<Judge>()
            .HasOne(j => j.PreviousTeam)
            .WithOne(t => t.PreviousJudge)
            .HasForeignKey<Judge>(j => j.PreviousTeamId);

        builder.Entity<Judge>()
            .HasMany(t => t.IgnoredTeams)
            .WithMany(j => j.JudgesIgnored)
            .UsingEntity(j => j.ToTable("JudgeIgnored"));

        builder.Entity<Judge>()
            .HasMany(t => t.ViewedTeams)
            .WithMany(j => j.JudgesViewed)
            .UsingEntity(j => j.ToTable("JudgeViewed"));
    }
}