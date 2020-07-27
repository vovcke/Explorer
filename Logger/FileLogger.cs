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
                File.AppendAllText(String.Format("{0} log.txt", DateTime.Now.ToString()), text);
            }
            catch (Exception)
            { }
        }

        public override void Shutdown()
        {
        }
    }
}
