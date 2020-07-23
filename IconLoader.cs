using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WpfExplorer
{
    public class IconLoader
    {
        #region Imports
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
        #endregion

        #region Singleton
        static IconLoader _instance;
        public static IconLoader GetInstance()
        {
            if (_instance == null)
            {
                _instance = new IconLoader();
            }
            return _instance;
        }
        #endregion

        Dictionary<string, BitmapSource> _fileIcons;
        public IconLoader()
        {
            _fileIcons = new Dictionary<string, BitmapSource>();
        }

        public BitmapSource GetIcon(string fileName)
        {
            if (_fileIcons.ContainsKey(fileName))
            {
                return _fileIcons[fileName];
            }

            SHFILEINFO info = new SHFILEINFO();
            IntPtr res = SHGetFileInfo(fileName, 0, ref info, (uint)Marshal.SizeOf(info), 0x101);

            BitmapSource src = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            _fileIcons[fileName] = src;

            return src;
        }

    }
}
