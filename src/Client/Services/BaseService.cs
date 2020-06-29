﻿using NoCrast.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class BaseService
    {
        public ILog Log { get; protected set; }

        public event EventHandler DataHasChanged;
        public event ServiceErrorEventHandler ErrorHasOccured;

        private bool isNetworkErrorRisen = false;
        private bool isUiErrorRisen = false;

        protected void NotifyDataHasChanged()
        {
            if (DataHasChanged != null)
            {
                DataHasChanged(this, new EventArgs());
            }
        }

        protected void NotifyUIError(Exception ex)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = ex.Message,
                IsUIError = true
            };

            Log.E(ex);
            NotifyError(arg);
            isUiErrorRisen = true;
        }

        protected void ResetUIError()
        {
            if (isUiErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    ResetUIError = true
                };
                NotifyError(arg);
                isUiErrorRisen = false;
            }
        }

        protected void ResetNetworkError()
        {
            if (isNetworkErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    ResetNetworkError = true
                };
                NotifyError(arg);
                isNetworkErrorRisen = false;
            }
        }

        protected void NotifyNetworkError(Exception ex)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = ex.Message,
                IsNetworkError = true
            };

            if(Log != null)
            {
                Log.E(ex);
            }

            NotifyError(arg);
            isNetworkErrorRisen = true;
        }

        private void NotifyError(ServiceErrorEventArgs arg)
        {
            if (ErrorHasOccured != null)
            {
                ErrorHasOccured(this, arg);
            }
        }
    }
}
