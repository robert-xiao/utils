using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ChargeStub.Utils
{
    class DataDispatcher<T>
    {
        private ConcurrentQueue<T> packages = new ConcurrentQueue<T>();
        private volatile bool exit = false;
        private Thread thread;
        private AutoResetEvent autoEvent = new AutoResetEvent(false);
        private Action<T> action;

        public DataDispatcher(Action<T> act)
        {
            action = act;
            thread = new Thread(Run);
        }
        public void Add(T a)
        {
            packages.Enqueue(a);
            autoEvent.Set();
        }

        public void Start()
        {
            exit = false;
            if (thread.ThreadState == ThreadState.Unstarted
                || thread.ThreadState == ThreadState.Stopped)
            {
                thread.Start();
            }
        }

        public void Stop()
        {
            exit = true;
            autoEvent.Set();
            if (thread.ThreadState == ThreadState.Running)
            {
                thread.Join();
            }
        }

        private void Run()
        {
            T rp;
            while (!exit)
            {
                if (packages.TryDequeue(out rp))
                {
                    action(rp);
                }
                else
                {
                    autoEvent.WaitOne();
                }
            }
        }
    }
}
