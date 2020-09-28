using NoCrast.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class ToolbarService
    {
        public event EventHandler ToolbarHasChanged;
        public event EventHandler Back;

        public string Title { get; private set; }
        public bool EnableBack { get { return Back != null; } }
        public bool EnableAddProject { get; set; }
        public bool EnableAddTag { get; set; }
        public bool EnableAddTask { get; set; }

        private ILog log;

        public ToolbarService(ILogProvider lp)
        {
            log = lp.CreateLogger(this);
        }

        public void Update(string title)
        {
            using(var l = log.DebugScope())
            {
                Title = title;
                ToolbarHasChanged?.Invoke(this, null);
            }
        }

        public void Update()
        {
            using (var l = log.DebugScope())
            {
                ToolbarHasChanged?.Invoke(this, null);
            }
        }

        public void InvokeBack()
        {
            using (var l = log.DebugScope())
            {
                Back?.Invoke(this, null);
            }
        }
    }
}
