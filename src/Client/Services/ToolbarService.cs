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
        public event EventHandler Add;

        public string Title { get; private set; }
        public bool IsBack { get { return Back != null; } }
        public bool IsAdd { get { return Add != null; } }

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
        public void InvokeBack()
        {
            using (var l = log.DebugScope())
            {
                Back?.Invoke(this, null);
            }
        }

        public void InvokeAdd()
        {
            using (var l = log.DebugScope())
            {
                Add?.Invoke(this, null);
            }
        }
    }
}
