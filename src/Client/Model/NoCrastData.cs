using NoCrast.Shared.Model;
using System.Collections.Generic;

namespace NoCrast.Client.Model
{
    public class NoCrastData
    {
        public const string StorageKeyName = "nocrast_data";
        public List<TaskItem> Tasks { get; set; }

        public NoCrastData()
        {
            Tasks = new List<TaskItem>();
        }
    }
}
