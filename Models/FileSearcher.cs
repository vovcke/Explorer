using System;
using System.Threading;
using WpfBrowser.Models.FileSearch;

namespace WpfBrowser.FileSearch
{
    public delegate void SearcherCallback(string filename);

    public class FileSearcher
    {
        #region Members
        SearcherStateContext Context;
        SearcherState State;

        public event SearcherCallback SearcherEvent;
        public event SearcherCallback SearchFinishedEvent;
        #endregion

        public FileSearcher()
        {
            Context = new SearcherStateContext();
            Context.SearcherThread = new Thread(ThreadStart);
            
            Context.SearcherThread.IsBackground = true;
            Context.SearcherThread.Start();

            State = new IdleState(Context);
        }

        public void Search(string path, string fileName)
        {
            lock(Context.Locker)
            {
                Context.Set(path, fileName, SearcherEvent, SearchFinishedEvent);
                Context.StartSearch = true;
                Context.SearchEvent.Set();
            }
        }

        void ThreadStart()
        {
            // State becomes null only after ExitState do his finalizing job
            while(State != null)
            {
                State = State.Work();
            }
        }

        public void Exit()
        {
            Context.Exit = true;
            Context.SearchEvent.Set();
        }

        public void StopSearch()
        {
            if(Context.IsBusy)
            {
                Context.StopSearch = true;
                Context.StartSearch = false;
                Context.SearchEvent.Set();
            }
        }
    }
}
