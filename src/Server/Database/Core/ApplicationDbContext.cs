using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoCrast.Server.Model;
using System;

namespace NoCrast.Server.Database
{
    public abstract class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<TimerTask> Tasks { get; set; }
        public DbSet<TimerTaskState> TaskState { get; set; }
        public DbSet<TimeLog> TimeLog { get; set; }
        public DbSet<TimerTag> TimerTags { get; set; }
        public DbSet<TagToTimerTask> TagToTimerTasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}