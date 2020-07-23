using System;
using System.IO;
using WpfBrowser.Models.FileSearch;

namespace WpfBrowser.FileSearch
{
    public enum StateName
    {
        Busy, Idle, Exit
    }
    abstract class SearcherState
    {
        protected SearcherStateContext Context;
        public SearcherState(SearcherStateContext ctx)
        {
            Context = ctx;
        }
        public abstract SearcherState Work();
        public abstract StateName GetName();
    }

    class IdleState : SearcherState
    {
        public IdleState(SearcherStateContext ctx) : base(ctx)
        {
        }

        public override StateName GetName() { return StateName.Idle; }

        public override SearcherState Work()
        {
            Context.SearchEvent.WaitOne();
            if (Context.Exit)
            {
                return new ExitState(Context);
            }
            else if (Context.Params != null)
            {
                Context.IsBusy = true;
                // If we are in an Idle state and get StopSearch command thats a logic error
                // Reset this flag
                Context.StopSearch = false;
                Context.StartSearch = false;
                return new BusyState(Context);
            }
            else if (Context.StopSearch)
            {
                Context.StopSearch = false;
            }
            return this;
        }
    }

    class BusyState : SearcherState
    {
        public BusyState(SearcherStateContext ctx) : base(ctx)
        {
        }
        public override StateName GetName() { return StateName.Busy; }

        public override SearcherState Work()
        {
            while (!Context.StopSearch && !Context.Exit)
            {
                if (Context.Exit)
                {
                    return new ExitState(Context);
                }

                SearchParams sparams = null;
                lock (Context.Locker)
                {
                    if (Context.StartSearch)
                    {
                        // We just woke up and havent started any search yet. We can get here if user starts new search
                        // while we havent started previous one. Just continue, context contains fresh input data
                        Context.StartSearch = false;
                    }

                    sparams = Context.Params;
                }

                DirectoryInfo info = new DirectoryInfo(sparams.Directory);

                if (DoSearch(info) == false)
                {
                    continue;
                }

                lock (Context.Locker)
                {
                    if(Context.Params.FinishCallback != null)
                    {
                        Context.Params.FinishCallback.Invoke("Success.");
                    }
                    // After search is done reset al the flags and data
                    Context.Reset();
                    Context.IsBusy = false;
                    Context.StopSearch = false;
                    Context.Params = null;
                }

                return new IdleState(Context);
            }
            if (Context.StopSearch)
            {
                lock (Context.Locker)
                {
                    Context.IsBusy = false;
                    Context.StopSearch = false;
                    Context.Params = null;
                }

                return new IdleState(Context);
            }
            else
            {
                return new ExitState(Context);
            }
        }
        bool DoSearch(DirectoryInfo info)
        {
            if (Context.StopSearch || Context.Exit)
                return false;
            try
            {
                Context.CurrentSearchDirectory = info.FullName;

                var dirs = info.EnumerateDirectories();
                var files = info.EnumerateFiles();

                foreach (var dir in dirs)
                {
                    if (Context.StopSearch || Context.Exit)
                        return false;
                    if (dir.Name.Contains(Context.Params.Name))
                    {
                        Context.Params.Callback.Invoke(dir.FullName.ToString());
                    }
                    if (DoSearch(dir) == false)
                    {
                        return false;
                    }
                }
                foreach (var file in files)
                {
                    if (Context.StopSearch || Context.Exit)
                        return false;
                    if (file.Name.Contains(Context.Params.Name))
                    {
                        Context.Params.Callback.Invoke(file.FullName.ToString());
                    }
                }
            }
            catch (Exception) { }
            return true;
        }
    }

    class ExitState : SearcherState
    {
        public ExitState(SearcherStateContext ctx) : base(ctx)
        {
        }

        public override StateName GetName() { return StateName.Exit; }
        public override SearcherState Work()
        {
            lock(Context.Locker)
            {
                Context.Exit = true;
                Context.SearchEvent.Set();
                Context.SearcherThread.Join(500);
            }

            return null;
        }
    }
}
