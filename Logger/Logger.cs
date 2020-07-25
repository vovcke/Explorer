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
#if DEBUG
                _instance = new WindowLogger();
#else
                _instance = new FileLogger();
#endif

            }
            return _instance;
        }
        #endregion

        public abstract void Log(string text);
        public abstract void Shutdown();
    }
}
