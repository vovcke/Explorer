using System;
using System.Windows;

namespace WpfExplorer
{
    public class WindowLogger : Logger
    {
        DebugWindow _debugWindow;

        public WindowLogger()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                _debugWindow = new DebugWindow();
                _debugWindow.Show();
            });
        }
        public override void Log(string text)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                _debugWindow.DebugInformation(text);
            });
        }

        public override void Shutdown()
        {
            _debugWindow.Close();
        }
    }
}
