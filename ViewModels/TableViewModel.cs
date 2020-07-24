using System;
using System.Windows;
using System.Windows.Input;
using WpfExplorer.Models;

namespace WpfExplorer.ViewModels
{
    public class TableViewModel : ExplorerViewModel
    {
        #region Commands
        private ICommand _itemClickedCommand;
        public ICommand ItemClickedCommand
        {
            get
            {
                return _itemClickedCommand ?? (_itemClickedCommand = new RelayCommand(x =>
                {
                    DirectoryItem item = x as DirectoryItem;
                    if (item != null)
                    {
                        Navigate(item.FullName);
                    }
                }));
            }
        }
        #endregion

        public TableViewModel(BrowserModel browser) : base(browser)
        {

        }
    }
}
