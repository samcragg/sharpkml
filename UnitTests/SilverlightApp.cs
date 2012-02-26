using System.Windows;

namespace UnitTests
{
    public sealed class SilverlightApp : Application
    {
        public SilverlightApp()
        {
            this.Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new SilverlightTestRunner();
        }
    }
}
