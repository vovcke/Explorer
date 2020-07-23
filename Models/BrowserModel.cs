using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
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
        FileSearcher Searcher;
        IconLoader _iconLoader;
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
            Searcher = new FileSearcher();
            _iconLoader = new IconLoader();

            Searcher.SearcherEvent += SearchCallback;
            Searcher.SearchFinishedEvent += SearchFinished;

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

            var dir = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dir.GetDirectories();

            _items.Clear();
            State = ExplorerState.Browsing;

            foreach (var val in dirs)
            {
                _items.Add(new DirectoryItem(val.Name, val.FullName, "", "Directory", ""));
            }
            FileInfo[] files = dir.GetFiles();
            foreach (var val in files)
            {
                _items.Add(
                    new DirectoryItem(
                        val.Name,
                        val.FullName,
                        val.Length.ToString(),
                        val.Extension.ToString(),
                        val.LastWriteTime.ToString()));
                
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
            Searcher.Search(path, fileName);
            if (StateChangedEvent != null)
            {
                StateChangedEvent.Invoke(State);
            }
        }

        void SearchCallback(string file)
        {
            if (State != ExplorerState.Searching)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (File.Exists(file))
                {
                    FileInfo val = new FileInfo(file);
                    Items.Add(
                        new DirectoryItem(
                            val.Name,
                            val.FullName,
                            val.Length.ToString(),
                            val.Extension.ToString(),
                            val.LastWriteTime.ToString()));
                }
                else if (Directory.Exists(file))
                {
                    DirectoryInfo info = new DirectoryInfo(file);
                    Items.Add(new DirectoryItem(info.Name, info.FullName, "", "Directory", ""));
                    
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
            Searcher.StopSearch();
            State = ExplorerState.SearchFinished;
            if (StateChangedEvent != null)
            {
                StateChangedEvent.Invoke(State);
            }
        }
    }
}
