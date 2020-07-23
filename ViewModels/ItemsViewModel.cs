using System;
using System.Windows.Input;
using WpfExplorer.Models;

namespace WpfExplorer.ViewModels
{
    public class ItemsViewModel : ExplorerViewModel
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
                        Navigate(item.Name);
                    }
                }));
            }
        }
        #endregion

        public ItemsViewModel(BrowserModel browser) : base(browser)
        {

        }
    }
}
