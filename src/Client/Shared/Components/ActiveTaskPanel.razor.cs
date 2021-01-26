using DevInstance.LogScope;
using DevInstance.TimelineLib;
using NoCrast.Shared.Model;
using System;

namespace NoCrast.Client.Shared.Components
{
    public partial class ActiveTaskPanel
    {
        private Timeline.Line[] Lines = new Timeline.Line[0];

        private ILog log;

        protected override void OnInitialized()
        {
            log = LogProvider.CreateLogger(this);
            using (var l = log.DebugExScope())
            {
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            using (var l = log.DebugExScope())
            {
                if(Items == null || TodayTimeLog == null)
                {
                    l.E("Error!!!");
                    return;
                }

                var lines = new Timeline.Line[Items.Length];
                for (var i = 0; i < Items.Length; i++)
                {
                    var itm = Items[i];
                    var log = TodayTimeLog[i];
                    var items = new Timeline.Item[log.Items.Length];
                    for (var j = 0; j < items.Length; j++)
                    {
                        items[j] = new Timeline.Item
                        {
                            StartTime = log.Items[j].StartTime.ToLocalTime(),
                            ElapsedTime = TimeSpan.FromMilliseconds(log.Items[j].ElapsedMilliseconds)
                        };
                    }
                    var line = new Timeline.Line {
                        Title = itm.Title,
                        CssClass = itm.Project != null ? itm.Project.Color.ToString().ToLower() : "white",
                        Descritpion = itm.Descritpion,
                        Items = items
                    };
                    lines[i] = line;
                }

                Lines = lines;
            }
            StateHasChanged();
        }

        private void Start(TaskItem item)
        {
            Service.StartTaskAsync(item);
        }

        private void Stop(TaskItem item)
        {
            Service.StopTaskAsync(item);
        }
    }
}
