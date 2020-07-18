using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoCrast.Server.Database;
using System;
using System.IO;
using System.Linq;
using NoCrast.Server.Model;

namespace NoCrast.ServerTests
{
    public class TestDatabase : IDisposable
    {
        public ApplicationDbContext db;

        public TestDatabase()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");

            DbContextOptionsBuilder<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseNpgsql(
                sqlConnectionString,
                b => b.MigrationsAssembly("NoCrast.Server")
            );

            db = new ApplicationDbContext(options.Options);
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                //Due to some bug in Npgsql we need to run
                //CREATE EXTENSION IF NOT EXISTS "uuid-ossp"
                throw new Exception("Migration failed. Make sure you run CREATE EXTENSION IF NOT EXISTS \"uuid - ossp\";", ex);
            }
            CleanUp();
        }

        public void Dispose()
        {
            CleanUp();
        }

        public void CleanUp()
        {
            db.TimeLog.RemoveRange((from items in db.TimeLog select items).ToList());
            db.SaveChanges();
            db.Tasks.RemoveRange((from items in db.Tasks select items).ToList());
            db.SaveChanges();
            db.UserProfiles.RemoveRange((from items in db.UserProfiles select items).ToList());
            db.SaveChanges();
        }

        public UserProfile profile;
        public TestDatabase UserProfile()
        {
            return UserProfile("nobody");
        }

        public TestDatabase UserProfile(string email)
        {
            profile = new UserProfile()
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = email,
                PublicId = email,
                ApplicationUserId = Guid.NewGuid(),
                Status = UserStatus.LIVE,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            db.UserProfiles.Add(profile);
            db.SaveChanges();
            return this;
        }

        public TimerTask lastTask;
        public TimerTaskState lastTaskState;
        public TestDatabase CreateTask(string name)
        {
            lastTask = new TimerTask
            {
                Id = Guid.NewGuid(),
                Profile = profile,
                PublicId = name,
                Title = name,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            lastTaskState = new TimerTaskState
            {
                Id = Guid.NewGuid(),
                Task = lastTask,
                IsRunning = false,
            };
            db.Tasks.Add(lastTask);
            db.TaskState.Add(lastTaskState);
            db.SaveChanges();
            return this;
        }

        public TimerTask FetchTask(string id)
        {
            return (from t in db.Tasks where t.PublicId == id select t).FirstOrDefault();
        }

        TimeLog lastLog;
        public TestDatabase CreateTimeLog(string name, bool startTask)
        {
            if(startTask)
            {
                lastTaskState.IsRunning = true;
            }

            lastLog = new TimeLog
            {
                Id = Guid.NewGuid(),
                Task = lastTask,
                StartTime = DateTime.Now,
                ElapsedMilliseconds = 0,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            db.TimeLog.Add(lastLog);
            db.SaveChanges();
            return this;
        }


    }
}