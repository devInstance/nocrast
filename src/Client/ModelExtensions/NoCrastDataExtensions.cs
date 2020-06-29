using NoCrast.Client.Model;
using NoCrast.Shared.Model;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.ModelExtensions
{
    public static class NoCrastDataExtensions
    {
        public static bool ApplyTaskItem(this NoCrastData data, TaskItem request, TaskItem response)
        {
            var taskId = request.GetInternalId();
 
            //TODO: optimize it later when logic is finalized
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                TaskItem task = data.Tasks[i];
                var id = task.GetInternalId();
                if (id.HasValue && id.Value == taskId
                    || !String.IsNullOrEmpty(task.Id) && request.Id == task.Id)
                {
                    data.Tasks[i] = response;
                    return true;
                }
            }
            return false;
        }

        public static bool InsertNewLog(this NoCrastData data, TaskItem item, TimeLogItem log)
        {
            var taskId = item.GetInternalId();

            //TODO: optimize it later when logic is finalized
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                TaskItem task = data.Tasks[i];
                var id = task.GetInternalId();
                if (id.HasValue && id.Value == taskId
                    || !String.IsNullOrEmpty(task.Id) && item.Id == task.Id)
                {
                    data.Logs[i].Add(log);
                    return true;
                }
            }
            return false;
        }

        public static bool ApplyStartTaskParameters(this NoCrastData data, UpdateTaskParameters request, UpdateTaskParameters response)
        {
            var taskId = request.Task.GetInternalId();
            var logId = request.Log.GetInternalId();

            //TODO: optimize it later when logic is finalized
            for (int i = 0; i < data.Tasks.Count; i ++)
            {
                TaskItem task = data.Tasks[i];
                var id = task.GetInternalId();
                if (id.HasValue && id.Value == taskId 
                    || !String.IsNullOrEmpty(task.Id) && request.Task.Id == task.Id)
                {
                    data.Tasks[i] = response.Task;
                }

                var log = data.Logs[i];
                //TODO: optimize it later when logic is finalized
                for (int n = 0; n < log.Count; n++)
                {
                    var l = log[n];
                    var lid = l.GetInternalId();
                    if (lid.HasValue && lid.Value == logId
                        || !String.IsNullOrEmpty(l.Id) && request.Log.Id == l.Id)
                    {
                        log[n] = response.Log;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
