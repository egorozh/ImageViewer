namespace ImageViewerSample
{
    internal class Logger : BaseViewModel, ILogger
    {
        public string LogMessage { get; set; }

        public void ShowMessage(string log)
        {
            LogMessage = log;
        }
    }
}