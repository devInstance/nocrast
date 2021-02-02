using DevInstance.LogScope;
using System;

namespace NoCrast.Client.Services
{
    public class BaseService
    {
        public IScopeLog Log { get; protected set; }

        protected NotificationService NotificationServ { get; }

        public BaseService(NotificationService notificationService)
        {
            this.NotificationServ = notificationService;
        }

        protected void NotifyDataHasChanged()
        {
            NotificationServ.NotifyDataHasChanged();
        }

        protected void NotifyUIError(string message)
        {
            NotificationServ.NotifyUIError(message);
        }

        protected void NotifyUIError(Exception ex)
        {
            NotificationServ.NotifyUIError(ex);
        }
        protected void ResetUIError()
        {
            NotificationServ.ResetUIError();
        }

        protected void ResetNetworkError()
        {
            NotificationServ.ResetNetworkError();
        }

        protected void NotifyNetworkError(Exception ex)
        {
            NotificationServ.NotifyNetworkError(ex);
        }

        private void NotifyError(ServiceErrorEventArgs arg)
        {
            NotificationServ.NotifyError(arg);
        }
    }
}
