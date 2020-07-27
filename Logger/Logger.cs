using System;
using System.Windows;
using WpfExplorer;

namespace WpfExplorer
{
    public abstract class Logger
    {
        #region Singleton members
        private static Logger _instance;
        public static Logger Instance()
        {
            if (_instance == null)
            {
                _instance = new FileLogger();
            }
            return _instance;
        }
        #endregion

        public abstract void Log(string text);
        public abstract void Shutdown();
    }
}
