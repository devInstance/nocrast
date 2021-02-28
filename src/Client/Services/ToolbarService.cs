using DevInstance.LogScope;

namespace NoCrast.Client.Services
{
    public delegate void ToolbarEventHandler(object value);

    public class ToolbarService
    {
        public event ToolbarEventHandler ToolbarHasChanged;
        public event ToolbarEventHandler Back;
        public event ToolbarEventHandler Delete;
        public event ToolbarEventHandler TitleChanged;

        public string Title { get; private set; }
        public bool EnableBack { get { return Back != null; } }
        public bool EnableAddProject { get; set; }
        public bool EnableAddTag { get; set; }
        public bool EnableAddTask { get; set; }
        public bool EnableDelete { get { return Delete != null; } }
        public bool EnableTitileChange { get { return TitleChanged != null; } }

        private IScopeLog log;

        public ToolbarService(IScopeManager lp)
        {
            log = lp.CreateLogger(this);
        }

        public void Update(string title)
        {
            using(var l = log.TraceScope())
            {
                Title = title;
                ToolbarHasChanged?.Invoke(null);
            }
        }

        public void Update()
        {
            using (var l = log.TraceScope())
            {
                ToolbarHasChanged?.Invoke(null);
            }
        }

        public void InvokeBack()
        {
            using (var l = log.TraceScope())
            {
                Back?.Invoke(null);
            }
        }

        public void InvokeDelete()
        {
            using (var l = log.TraceScope())
            {
                Delete?.Invoke(null);
            }
        }

        public void InvokeTitleChanged(string value)
        {
            using (var l = log.TraceScope())
            {
                Title = value;
                TitleChanged?.Invoke(value);
            }
        }
    }
}
