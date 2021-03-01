using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevInstance.Timelines;
using NoCrast.Client.ModelExtensions;
using NoCrast.Shared.Utils;

namespace NoCrast.Client.Utils
{
    public static class TimelinesUtils
    {
        public static Timeline.Line[] GetLines(ITimeProvider timeProvider, TaskItem[] items, ModelList<TimeLogItem>[] todayTimeLog)
        {
            if (items == null || todayTimeLog == null)
            {
                return null;
            }

            try
            {
                var lines = new Timeline.Line[items.Length];
                for (var i = 0; i < items.Length; i++)
                {
                    var itm = items[i];
                    var log = todayTimeLog[i];

                    var titems = new Timeline.Item[log.Items.Length];

                    for (var j = 0; j < titems.Length; j++)
                    {
                        titems[j] = new Timeline.Item
                        {
                            StartTime = log.Items[j].StartTime.ToLocalTime(),
                            ElapsedTime = log.Items[j].GetElapsedTimeSpan(timeProvider)
                        };
                    }
                    var line = new Timeline.Line
                    {
                        Title = itm.Title,
                        CssClass = itm.Project != null ? itm.Project.Color.ToString().ToLower() : "white",
                        Descritpion = itm.Descritpion,
                        Items = titems
                    };
                    lines[i] = line;
                }

                return lines;
            }
            catch(Exception ex)
            {
            }
            return null;
        }
    }
}
