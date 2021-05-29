using System.Windows;

namespace ImageViewerSample
{
    internal partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IoC.Instance.MainWindow.Show();
        }
    }
}