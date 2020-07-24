using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WpfExplorer.Models;
using WpfExplorer.Views;

namespace WpfExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Members
        private Dictionary<string, ExplorerViewModel> ViewModels;
        BrowserModel Browser;

        private ExplorerViewModel _currentViewModel;
        public ExplorerViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        public List<string> ViewModelNames
        {
            get
            {
                return new List<string>(ViewModels.Keys);
            } 
        }

        public string CurrentViewModelName
        {
            get
            {
                return CurrentViewModel.ToString();
            }
            set
            {
                ChangeViewModel(value);
                OnPropertyChanged("CurrentViewModelName");
            }
        }

        private string _pathTextBox = "";
        public string PathTextBox
        {
            get
            {
                return _pathTextBox;
            }
            set
            {
                _pathTextBox = value;
                OnPropertyChanged("PathTextBox");
            }
        }

        private string _searchTextBox = "";
        public string SearchTextBox
        {
            get
            {
                return _searchTextBox;
            }
            set
            {
                _searchTextBox = value;
                OnPropertyChanged("SearchTextBox");
            }
        }

        private string _statusTextBox = "";
        public string StatusTextBox
        {
            get
            {
                return _statusTextBox;
            }
            set
            {
                _statusTextBox = value;
                OnPropertyChanged("StatusTextBox");
            }
        }
        public Visibility IsSearching
        {
            get
            {
                return (Browser.State == BrowserModel.ExplorerState.Searching) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public Visibility IsBrowsing
        {
            get
            {
                return (Browser.State != BrowserModel.ExplorerState.Searching) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        #endregion

        private void ChangeViewModel(string name)
        {
            if (!ViewModels.ContainsKey(name))
            {
                throw new Exception("Error");
            }

            CurrentViewModel = ViewModels[name];
            OnPropertyChanged("Items");
        }
        
        #region Commands
        private ICommand _navigateCommand;
        public ICommand NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new RelayCommand(x =>
                {
                    StatusTextBox = "";
                    CurrentViewModel.Navigate(PathTextBox);
                }));
            }
        }

        private ICommand _navigateUpCommand;
        public ICommand NavigateUpCommand
        {
            get
            {
                return _navigateUpCommand ?? (_navigateUpCommand = new RelayCommand(x =>
                {
                    StatusTextBox = "";
                    PathTextBox = CurrentViewModel.NavigateUp();
                }));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand(x =>
                {
                    CurrentViewModel.Search(PathTextBox, SearchTextBox);
                }));
            }
        }

        private ICommand _stopSearchCommand;
        public ICommand StopSearchCommand
        {
            get
            {
                return _stopSearchCommand ?? (_stopSearchCommand = new RelayCommand(x =>
                {
                    CurrentViewModel.StopSearch();
                }));
            }
        }

        private ICommand _pathTextBoxEnterCommand;
        public ICommand PathTextBoxEnterCommand
        {
            get
            {
                return _pathTextBoxEnterCommand ?? (_pathTextBoxEnterCommand = new RelayCommand(x =>
                {
                    StatusTextBox = "";
                    string param = x as string;
                    CurrentViewModel.Navigate(param);
                }));
            }
        }

        private ICommand _searchTextBoxEnterCommand;
        public ICommand SearchTextBoxEnterCommand
        {
            get
            {
                return _searchTextBoxEnterCommand ?? (_searchTextBoxEnterCommand = new RelayCommand(x =>
                {
                    string param = x as string;
                    CurrentViewModel.Search(PathTextBox, param);
                }));
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            ViewModels = new Dictionary<string, ExplorerViewModel>();

            Browser = new BrowserModel();
            // Add available pages and set page
            ViewModels.Add("Table", new TableViewModel(Browser));
            ViewModels.Add("Items", new ItemsViewModel(Browser));

            CurrentViewModel = ViewModels["Table"];

            PathTextBox = "C:\\";
            CurrentViewModel.Navigate(PathTextBox);

            foreach(var elem in ViewModels.Values)
            {
                elem.PropertyChanged += ModelPropertyChanged;
            }
            Browser.StateChangedEvent += Browser_StateChangedEvent;
            Browser.SearcherProgressEvent += Browser_SearcherProgressEvent;
            Browser.SearcherFinishedEvent += Browser_SearcherFinishedEvent; ;
        }
        private void Browser_SearcherFinishedEvent(string filename)
        {
            StatusTextBox = filename;
        }

        private void Browser_SearcherProgressEvent(string filename)
        {
            StatusTextBox = filename;
        }

        private void Browser_StateChangedEvent(BrowserModel.ExplorerState state)
        {
            OnPropertyChanged("IsSearching");
            OnPropertyChanged("IsBrowsing");
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentDirectory")
            {
                PathTextBox = CurrentViewModel.CurrentDirectory;
            }
        }
    }
}
