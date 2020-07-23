using System;
using System.IO;
using System.Windows;
using WpfExplorer.Models;

namespace WpfExplorer.ViewModels
{
    public class ExplorerViewModel : ViewModelBase
    {
        #region Members
        public string CurrentDirectory 
        {
            get
            {
                return Browser.CurrentDirectory;
            }
        }
        public BrowserModel Browser { get; private set; }
        #endregion

        public ExplorerViewModel(BrowserModel browser)
        {
            Browser = browser;
        }

        #region Navigate
        public void Navigate(string path)
        {
            if (path.Length == 0)
                return;

            string fullName = CheckPath(path);
            if (fullName.Length == 0)
            {
                if(TryOpenFile(path))
                {
                    MessageBox.Show("Invalid path.");
                }
                return;
            }

            DoNavigate(fullName);
        }
        #endregion

        private void DoNavigate(string path)
        {
            Browser.Browse(path);
            OnPropertyChanged("CurrentDirectory");
        }

        private string CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                string fullPath = Path.Combine(Browser.CurrentDirectory, path);
                if (!Directory.Exists(fullPath))
                {
                    return "";
                }
                return fullPath;
            }
            return path;
        }

        bool TryOpenFile(string fileName)
        {
            string file = fileName;
            if (!File.Exists(fileName))
            {
                string fullPath = Path.Combine(Browser.CurrentDirectory, fileName);
                if (!File.Exists(fullPath))
                {
                    return false;
                }
                file = fullPath;
            }
            Util.Util.OpenFile(file);
            return true;
        }

        public bool CanNavigateUp()
        {
            return CheckPath(Path.GetFullPath(Path.Combine(Browser.CurrentDirectory, @"..\"))) != "";
        }

        public string NavigateUp()
        {
            if (!CanNavigateUp())
                return "";

            string newPath = CheckPath(Path.GetFullPath(Path.Combine(Browser.CurrentDirectory, @"..\")));

            if (newPath.Length != 0)
            {
                DoNavigate(newPath);
            }
            return CurrentDirectory;
        }

        public void Search(string path, string text)
        {
            Browser.Search(path, text);
        }

        public void StopSearch()
        {
            Browser.StopSearch();
        }
    }
}
