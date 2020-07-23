using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfBrowser.FileSearch;

namespace WpfExplorer.Models
{
    public class BrowserModel : INotifyPropertyChanged
    {
        public enum ExplorerState
        {
            Browsing, Searching, SearchFinished
        }

        #region Members
        private ObservableCollection<DirectoryItem> _items;
        public ObservableCollection<DirectoryItem> Items
        {
            get { return _items; }
            protected set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }
        FileSearcher searcher;
        public string CurrentDirectory { get; private set; }
        public ExplorerState State { get; private set; }

        public delegate void StateChangedDelegate(ExplorerState state);
        public event StateChangedDelegate StateChangedEvent;

        #endregion

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        public BrowserModel()
        {
            CurrentDirectory = "";
            searcher = new FileSearcher();
            _items = new ObservableCollection<DirectoryItem>();
            State = ExplorerState.Browsing;
        }

        public bool Browse(string path)
        {
            if (path.Length == 0)
                return false;

            if (State == ExplorerState.Searching)
            {
                StopSearch();
            }
            else if (CurrentDirectory.CompareTo(path) == 0 && State == ExplorerState.Browsing)
            {
                return true;
            }

            _items.Clear();

            var dir = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (var val in dirs)
            {
                _items.Add(new DirectoryItem()
                {
                    Name = val.Name,
                    Size = "",
                    Type = "Directory",
                    LastModify = ""
                });
            }
            FileInfo[] files = dir.GetFiles();
            foreach (var val in files)
            {
                _items.Add(new DirectoryItem()
                {
                    Name = val.Name,
                    Size = val.Length.ToString(),
                    Type = val.Extension.ToString(),
                    LastModify = val.LastWriteTime.ToString()
                });
            }
            CurrentDirectory = path;
            OnPropertyChanged("Items");
            OnPropertyChanged("CurrentDirectory");
            return true;
        }

        public void Search(string path, string fileName)
        {
            State = ExplorerState.Searching;
            Items.Clear();
            searcher.Search(path, fileName, SearchCallback, SearchFinished);
            if (StateChangedEvent != null)
            {
                StateChangedEvent.Invoke(State);
            }
        }

        void SearchCallback(string val)
        {
            if (State != ExplorerState.Searching)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (File.Exists(val))
                {
                    FileInfo file = new FileInfo(val);
                    Items.Add(new DirectoryItem()
                    {
                        Name = val,
                        Size = file.Length.ToString(),
                        Type = file.Extension.ToString(),
                        LastModify = file.LastWriteTime.ToString()
                    });
                }
                else if (Directory.Exists(val))
                {
                    Items.Add(new DirectoryItem()
                    {
                        Name = val,
                        Size = "",
                        Type = "Directory",
                        LastModify = ""
                    });
                }
            });
        }

        void SearchFinished(string fileName)
        {
            MessageBox.Show("Search finished.");
            State = ExplorerState.SearchFinished;
            if (StateChangedEvent != null)
            {
                StateChangedEvent.Invoke(State);
            }
        }

        public void StopSearch()
        {
            searcher.StopSearch();
            State = ExplorerState.SearchFinished;
            if (StateChangedEvent != null)
            {
                StateChangedEvent.Invoke(State);
            }
        }
    }
}
