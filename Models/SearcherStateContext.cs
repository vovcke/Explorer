using System;
using System.Threading;
using WpfBrowser.FileSearch;

namespace WpfBrowser.Models.FileSearch
{
    class SearchParams
    {
        #region Members
        public string Directory;
        public string Name;
        public SearcherCallback Callback;
        public SearcherCallback FinishCallback;
        #endregion

        public SearchParams(string path, string name, SearcherCallback callb, SearcherCallback fcallb)
        {
            Directory = path;
            Name = name;
            Callback = callb;
            FinishCallback = fcallb;
        }
    }

    class SearcherStateContext
    {
        #region Members
        public SearchParams Params;
        public object Locker = new object();
        public bool IsBusy = false;
        public bool StartSearch = false;
        public bool StopSearch = false;
        public bool Exit = false;
        public AutoResetEvent SearchEvent = new AutoResetEvent(false);

        public Thread SearcherThread;
        #endregion
        public SearcherStateContext()
        {

        }

        public void Set(string path, string name, SearcherCallback callb, SearcherCallback fcallb)
        {
            Reset();
            Params = new SearchParams(path, name, callb, fcallb);
        }

        public void Reset()
        {
            IsBusy = false;
            StopSearch = false;
            Exit = false;
            Locker = new object();
        }
    }
}
