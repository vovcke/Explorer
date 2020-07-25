using System;
using System.Threading;
using WpfBrowser.Models.FileSearch;
using WpfExplorer;

namespace WpfBrowser.FileSearch
{
    public delegate void SearcherCallback(string filename);

    public class FileSearcher
    {
        #region Members
        SearcherStateContext Context;
        SearcherState State;
        System.Timers.Timer ProgressTimer;

        public event SearcherCallback SearcherEvent;
        public event SearcherCallback SearchFinishedEvent;
        public event SearcherCallback SearchProgressEvent;
        #endregion

        public FileSearcher()
        {
            Context = new SearcherStateContext();
            Context.SearcherThread = new Thread(ThreadStart);

            ProgressTimer = new System.Timers.Timer(200);
            ProgressTimer.Elapsed += OnProgress;

            Context.SearcherThread.IsBackground = true;
            Context.SearcherThread.Start();

            SearchFinishedEvent += FileSearcher_SearchFinishedEvent;

            State = new IdleState(Context);
        }

        public void Search(string path, string fileName)
        {
            Logger.Instance().Log(
                String.Format("public void Search({0}, {1})", path, fileName));
            lock (Context.Locker)
            {
                Context.Set(path, fileName, SearcherEvent, SearchFinishedEvent);
                Context.StartSearch = true;
                Context.SearchEvent.Set();
                ProgressTimer.Start();
            }
        }

        void ThreadStart()
        {
            Logger.Instance().Log("void ThreadStart()");
            // State becomes null only after ExitState do his finalizing job
            while (State != null)
            {
                State = State.Work();
            }
        }

        public void Exit()
        {
            ProgressTimer.Stop();
            Context.Exit = true;
            Context.SearchEvent.Set();
        }

        public void StopSearch()
        {
            if (Context.IsBusy)
            {
                ProgressTimer.Stop();
                Context.StopSearch = true;
                Context.StartSearch = false;
                Context.SearchEvent.Set();
            }
        }

        private void OnProgress(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (SearchProgressEvent != null)
            {
                SearchProgressEvent.Invoke(Context.CurrentSearchDirectory);
            }
        }

        private void FileSearcher_SearchFinishedEvent(string filename)
        {
            ProgressTimer.Stop();
        }
    }
}
