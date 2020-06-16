namespace NoCrast.Client.Services
{
    public delegate void ServiceErrorEventHandler(object sender, ServiceErrorEventArgs e);

    public class ServiceErrorEventArgs
    {
        public bool IsNetworkError { get; set; }

        public bool IsUIError { get; set; }

        public bool ResetNetworkError { get; set; }

        public bool ResetUIError { get; set; }

        public string Message { get; set; }
    }
}