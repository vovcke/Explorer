using System;
using System.Diagnostics;
using System.Windows;

namespace WpfExplorer.Util
{
    public class Util
    {
        public static void OpenFile(string file)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(file);
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
