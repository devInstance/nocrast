using NoCrast.Shared.Model;
using System;
using System.Runtime.CompilerServices;

namespace NoCrast.Client.ModelExtensions
{
    public static class DataItemExtender
    {
        class ClientModelAttributes
        {
            public Guid internalId { get; set; }
        }

        static readonly ConditionalWeakTable<TaskItem, ClientModelAttributes> TaskItem_ClientModelAttributes = new ConditionalWeakTable<TaskItem, ClientModelAttributes>();

        public static Guid? GetInternalId(this TaskItem originalObj) 
        {
            ClientModelAttributes value;
            if (TaskItem_ClientModelAttributes.TryGetValue(originalObj, out value))
            {
                return value.internalId;
            }
            return null;
        }

        public static void SetInternalId(this TaskItem originalObj, Guid id) 
        { 
            TaskItem_ClientModelAttributes.GetOrCreateValue(originalObj).internalId = id;
        }

        static readonly ConditionalWeakTable<TimeLogItem, ClientModelAttributes> TimeLogItem_ClientModelAttributes = new ConditionalWeakTable<TimeLogItem, ClientModelAttributes>();

        public static Guid? GetInternalId(this TimeLogItem originalObj)
        {
            ClientModelAttributes value;
            if (TimeLogItem_ClientModelAttributes.TryGetValue(originalObj, out value))
            {
                return value.internalId;
            }
            return null;
        }

        public static void SetInternalId(this TimeLogItem originalObj, Guid id) 
        { 
            TimeLogItem_ClientModelAttributes.GetOrCreateValue(originalObj).internalId = id;
        }
    }
}
