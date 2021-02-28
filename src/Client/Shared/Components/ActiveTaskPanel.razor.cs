using DevInstance.LogScope;
using static DevInstance.Timelines.Timeline;
using NoCrast.Shared.Model;
using System;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.Utils;

namespace NoCrast.Client.Shared.Components
{
    public partial class ActiveTaskPanel
    {
        private Line[] Lines => TimelinesUtils.GetLines(TimeProvider, Items, TodayTimeLog);

        private IScopeLog log;

        protected override void OnInitialized()
        {
            log = ScopeManager.CreateLogger(this);
            using (var l = log.TraceScope())
            {
            }
        }

        protected override void OnParametersSet()
        {
            using (var l = log.TraceScope())
            {
                base.OnParametersSet();

                StateHasChanged();
            }
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
