using NoCrast.Client.ModelExtensions;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;

namespace NoCrast.Client.Services
{
    public class TimersService
    {
        private List<TaskItem> list = new List<TaskItem>();

        public ITimeProvider Provider { get; }
        public ILog Log { get; private set; }

        public event EventHandler DataHasChanged;

        public TimersService(ITimeProvider provider, ILogProvider logProvider)
        {
            LogTEst.MainF();

            Provider = provider;
            Log = logProvider.CreateLogger(this);

            LogTEst.MainF();
        }

        private void NotifyDataHasChanged()
        {
            if (DataHasChanged != null)
            {
                DataHasChanged(this, new EventArgs());
            }
        }

        public List<TaskItem> GetTasks()
        {
            using(var l = Log.DebugScope())
            {
                return list;
            }
        }

        public TaskItem AddNewTask(string title)
        {
            var task = new TaskItem { Title = title };
            list.Add(task);

            NotifyDataHasChanged();
            
            return task;
        }

        public void RemoveTask(TaskItem item)
        {
            list.Remove(item);

            NotifyDataHasChanged();
        }

        public void StartTask(TaskItem item)
        {
            item.Start(Provider);

            NotifyDataHasChanged();
        }

        public void StopTask(TaskItem item)
        {
            item.Stop(Provider);

            NotifyDataHasChanged();
        }
    }
}
