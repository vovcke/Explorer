using System;
using System.IO;

namespace WpfExplorer
{
    public class FileLogger : Logger
    {
        public FileLogger()
        {

        }

        public override void Log(string text)
        {
            try
            {
                File.AppendAllText("log.txt", text);
            }
            catch (Exception)
            { }
        }

        public override void Shutdown()
        {
        }
    }
}
