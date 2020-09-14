using NoCrast.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class NotificationService
    {
        public ILog Log { get; protected set; }

        public event EventHandler DataHasChanged;
        public event ServiceErrorEventHandler ErrorHasOccured;

        private bool isNetworkErrorRisen = false;
        private bool isUiErrorRisen = false;

        public void NotifyDataHasChanged()
        {
            DataHasChanged?.Invoke(this, new EventArgs());
        }

        public void NotifyUIError(string message)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = message,
                IsUIError = true
            };

            Log.E(message);
            NotifyError(arg);
            isUiErrorRisen = true;
        }

        public void NotifyUIError(Exception ex)
        {
            if (!isUiErrorRisen)
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
        }
        public void ResetUIError()
        {
            if (isUiErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    IsUIError = false
                };
                NotifyError(arg);
                isUiErrorRisen = false;
            }
        }

        public void ResetNetworkError()
        {
            if (isNetworkErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    IsNetworkError = false
                };
                NotifyError(arg);
                isNetworkErrorRisen = false;
            }
        }

        public void NotifyNetworkError(Exception ex)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = ex.Message,
                IsNetworkError = true
            };

            if (Log != null)
            {
                Log.E(ex);
            }

            NotifyError(arg);
            isNetworkErrorRisen = true;
        }

        public void NotifyError(ServiceErrorEventArgs arg)
        {
            ErrorHasOccured?.Invoke(this, arg);
        }
    }
}
