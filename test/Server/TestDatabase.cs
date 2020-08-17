using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoCrast.Server.Database;
using System;
using System.IO;
using System.Linq;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;

namespace NoCrast.ServerTests
{
    public class TestDatabase : IDisposable
    {
        ITimeProvider TimeProvider { get; set; }
        public ApplicationDbContext db;

        public TestDatabase(ITimeProvider provider)
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

            TimeProvider = provider;
        }

        public void Dispose()
        {
            CleanUp();
        }

        public void CleanUp()
        {
            db.TaskState.RemoveRange((from items in db.TaskState select items).ToList());
            db.SaveChanges();
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
                CreateDate = TimeProvider.CurrentTime,
                UpdateDate = TimeProvider.CurrentTime,
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
                CreateDate = TimeProvider.CurrentTime,
                UpdateDate = TimeProvider.CurrentTime,
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
        public TestDatabase CreateTimeLog(DateTime startTime, long elapsedMilliseconds, bool startTask)
        {
            lastLog = new TimeLog
            {
                Id = Guid.NewGuid(),
                PublicId = Guid.NewGuid().ToString(),
                Task = lastTask,
                StartTime = startTime,
                ElapsedMilliseconds = elapsedMilliseconds,
                CreateDate = TimeProvider.CurrentTime,
                UpdateDate = TimeProvider.CurrentTime,
            };
            db.TimeLog.Add(lastLog);
            if (startTask)
            {
                lastTaskState.IsRunning = true;
                lastTaskState.ActiveTimeLogItem = lastLog;
            }
            db.SaveChanges();
            return this;
        }


    }
}