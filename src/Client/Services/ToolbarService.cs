﻿using NoCrast.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                ToolbarHasChanged?.Invoke(null);
            }
        }

        public void Update()
        {
            using (var l = log.DebugScope())
            {
                ToolbarHasChanged?.Invoke(null);
            }
        }

        public void InvokeBack()
        {
            using (var l = log.DebugScope())
            {
                Back?.Invoke(null);
            }
        }

        public void InvokeDelete()
        {
            using (var l = log.DebugScope())
            {
                Delete?.Invoke(null);
            }
        }

        public void InvokeTitleChanged(string value)
        {
            using (var l = log.DebugScope())
            {
                Title = value;
                TitleChanged?.Invoke(value);
            }
        }
    }
}