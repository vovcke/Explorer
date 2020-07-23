using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WpfExplorer.ViewModels;

namespace WpfExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SHGetFileInfo(string path, UInt32 attrs, ref SHFILEINFO info, UInt32 size, UInt32 flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttrs;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string displayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string typeName;
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
