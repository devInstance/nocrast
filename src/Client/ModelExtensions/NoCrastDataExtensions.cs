using NoCrast.Client.Model;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;

namespace NoCrast.Client.ModelExtensions
{
    public static class NoCrastDataExtensions
    {
        public static int FindTaskIndex(this NoCrastData data, TaskItem item)
        {
            if(!String.IsNullOrEmpty(item.Id))
            {
                var index = data.Tasks.FindIndex(f => f.Id == item.Id);
                if (index >= 0)
                {
                    return index;
                }
            }
            return data.Tasks.FindIndex(f => f.ClientId == item.ClientId);
        }

        public static int FindTimeLogIndex(this NoCrastData data, int taskIndex, TimeLogItem item)
        {
            var logs = data.Logs[taskIndex];
            if (!String.IsNullOrEmpty(item.Id))
            {
                var index = logs.FindIndex(f => f.Id == item.Id);
                if(index >= 0)
                {
                    return index;
                }
            }
            return logs.FindIndex(f => f.ClientId == item.ClientId);
        }

        public static bool ApplyTaskItem(this NoCrastData data, TaskItem response)
        {
            var index = data.FindTaskIndex(response);
            if(index < 0)
            {
                return false;
            }
            data.Tasks[index] = response;

            return true;
        }

        public static bool InsertNewLog(this NoCrastData data, TaskItem item, TimeLogItem log)
        {
            var index = data.FindTaskIndex(item);
            if (index < 0)
            {
                return false;
            }

            data.Logs[index].Add(log);

            return true;
        }

        public static bool UpdateTimeLog(this NoCrastData data, TaskItem item, TimeLogItem log)
        {
            var index = data.FindTaskIndex(item);
            if (index < 0)
            {
                return false;
            }

            var logIndex = data.FindTimeLogIndex(index, log);
            if (logIndex < 0)
            {
                return false;
            }

            data.Logs[index][logIndex] = log;

            return true;
        }

        public static bool ApplyStartTaskParameters(this NoCrastData data, TaskItem item, TimeLogItem log)
        {
            var index = data.FindTaskIndex(item);
            if (index < 0)
            {
                return false;
            }

            //TODO:item.ClientId = null;
            data.Tasks[index] = item;

            var logIndex = data.FindTimeLogIndex(index, log);
            if (logIndex < 0)
            {
                return false;
            }
            //TODO:log.ClientId = null;
            data.Logs[index][logIndex] = log;
            
            return true;
        }

        public static bool ApplyTimeLog(this NoCrastData data, TaskItem task, List<TimeLogItem> response)
        {
            throw new NotImplementedException();
        }

    }
}
