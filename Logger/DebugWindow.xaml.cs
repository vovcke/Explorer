using System;
using System.Diagnostics;
using System.Windows;

namespace WpfExplorer
{
    /// <summary>
    /// Логика взаимодействия для DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        public void DebugInformation(string text)
        {
            debugTbx.Text += text + Environment.NewLine;
        }
    }
}
