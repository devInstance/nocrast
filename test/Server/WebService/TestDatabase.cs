using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoCrast.Server.Database;
using System;
using System.IO;
using System.Linq;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System.Collections.Generic;
using NoCrast.Shared.Model;
using NoCrast.Server.Database.Postgres;

namespace NoCrast.ServerTests
{
    public class TestDatabase : IDisposable
    {
        ITimeProvider TimeProvider { get; set; }
        public ApplicationDbContext db;

        private bool isEndSetupCalled = false;

        public TestDatabase(ITimeProvider provider)
        {
            SetupDBContext();

            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                //Due to some bug in Npgsql we need to run
                //CREATE EXTENSION IF NOT EXISTS "uuid-ossp"
                throw new Exception("Migration failed. Make sure you run CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";", ex);
            }
            CleanUp();

            TimeProvider = provider;
        }

        /// <summary>
        /// Resets DB context. Has to be called in the end of every setup.
        /// It will help uncover issues related to object in memory behaviors of EF Core
        /// such as cascade deletes which behaves differently in the test and real world scenario
        /// </summary>
        public void EndSetup()
        {
            isEndSetupCalled = true;
            SetupDBContext();
        }

        public void SetupDBContext()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var sqlConnectionString = configuration.GetConnectionString("UntiTestConnection");

            DbContextOptionsBuilder<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseNpgsql(
                sqlConnectionString,
                b => b.MigrationsAssembly("NoCrast.Server")
            );

            db = new PostgresApplicationDbContext(options.Options);
        }

        public void Dispose()
        {
            CleanUp();
            if (!isEndSetupCalled) throw new Exception("You must call 'EndSetup' for every test");
        }

        public void CleanUp()
        {
            db.TagToTimerTasks.RemoveRange((from items in db.TagToTimerTasks select items).ToList());
            db.SaveChanges();
            db.TimerTags.RemoveRange((from items in db.TimerTags select items).ToList());
            db.SaveChanges();
            db.TaskState.RemoveRange((from items in db.TaskState select items).ToList());
            db.SaveChanges();
            db.TimeLog.RemoveRange((from items in db.TimeLog select items).ToList());
            db.SaveChanges();
            db.Tasks.RemoveRange((from items in db.Tasks select items).ToList());
            db.SaveChanges();
            db.Projects.RemoveRange((from items in db.Projects select items).ToList());
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

        public UserProfile FetchUser(string id)
        {
            return (from t in db.UserProfiles where t.Email == id select t).FirstOrDefault();
        }

        public List<UserProfile> FetchAllUsers(string id)
        {
            return (from t in db.UserProfiles where id == null || t.Email == id select t).ToList();
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
                Project = lastProject,
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

        public Project lastProject = null;
        public TestDatabase CreateProject(string name)
        {
            return CreateProject(name, ProjectColor.Rose);
        }

        public TestDatabase CreateProject(string name, ProjectColor color)
        {
            lastProject = new Project
            {
                Id = Guid.NewGuid(),
                Profile = profile,
                PublicId = name,
                Title = name,
                Color = color,
                CreateDate = TimeProvider.CurrentTime,
                UpdateDate = TimeProvider.CurrentTime,
            };

            db.Projects.Add(lastProject);
            db.SaveChanges();
            return this;
        }

        public Project FetchProject(string id)
        {
            return (from t in db.Projects where t.PublicId == id select t).FirstOrDefault();
        }

        public TimeLog lastLog;
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

        public TimeLog FetchTimeLog(string id)
        {
            return (from t in db.TimeLog where t.PublicId == id select t).FirstOrDefault();
        }

        public TimerTag lastTag;
        public TestDatabase CreateTag(string name)
        {
            lastTag = new TimerTag
            {
                Id = Guid.NewGuid(),
                PublicId = Guid.NewGuid().ToString(),
                Profile = profile,

                Name = name,

                CreateDate = TimeProvider.CurrentTime,
                UpdateDate = TimeProvider.CurrentTime,
            };

            db.TimerTags.Add(lastTag);
            db.SaveChanges();

            return this;
        }

        public TimerTag FetchTag(string id)
        {
            return (from t in db.TimerTags where t.PublicId == id select t).FirstOrDefault();
        }

        public TestDatabase AssignLastTag()
        {
            var taskToTag = new TagToTimerTask
            {
                Id = Guid.NewGuid(),
                Tag = lastTag,
                Task = lastTask
            };

            db.TagToTimerTasks.Add(taskToTag);
            db.SaveChanges();

            return this;
        }
    }
}