using System;
using System.Collections.Generic;

namespace ChargeStub.Utils
{
    abstract class DataDispatcherPool<T>
    {
        protected int concurrents;
        private List<DataDispatcher<T> > pools = new List<DataDispatcher<T> >();
        private Action<T> action;

        protected abstract int CalcWhich(T t);
        protected DataDispatcherPool(int amount, Action<T> act)
        {
            concurrents = amount;
            for (int i = 0; i < concurrents; ++i)
            {
                pools.Add(null);
            }
            action = act;
        }

        public void Init()
        {
            lock (pools)
            {
                for (int i = 0; i < concurrents; ++i)
                {
                    DataDispatcher<T> rps = new DataDispatcher<T>(action);
                    rps.Start();
                    pools[i] = rps; 
                }
            }

        }

        public void Stop()
        {
            for (int i = 0; i < concurrents; ++i)
            {
                DataDispatcher<T> rps = pools[i];
                if (rps != null)
                {
                    rps.Stop();
                }
                pools[i] = null;
            }
        }

        public void Add(T a)
        {
            int which = CalcWhich(a);
            which = Math.Abs(which);
            which = which % concurrents;
            DataDispatcher<T> rps = pools[which];
            if(rps != null)
            {
                rps.Add(a);
            }
        }
    }
}
