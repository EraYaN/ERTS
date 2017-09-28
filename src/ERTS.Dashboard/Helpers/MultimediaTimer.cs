using System;
using System.Runtime.InteropServices;

namespace ERTS.Dashboard.Helpers
{
    public class MultimediaTimer : IDisposable
    {
        private uint fastTimer;
        private bool isrunning = false;
        private TimerEventHandler handler;
        //private int count;
        //private int maxCount;
        private int interval;
        public uint MaximumInterval;
        public uint MinimumInterval;

        public event EventHandler<EventArgs> Elapsed;

        public MultimediaTimer(int msDelay = 1)
        {
            interval = msDelay;
            GetCapabilities(out uint MinimumInterval, out uint MaximumInterval);
            handler = new TimerEventHandler(TickHandler);
        }

        public void Start()
        {
            if (isrunning)
                Stop();
            isrunning = true;
            int myData = 0;
            fastTimer = timeSetEvent(interval, interval, handler, ref myData, 1);
        }

        public void Stop()
        {
            if (isrunning)
            {
                timeKillEvent(fastTimer);
            }
        }
        

        private void GetCapabilities(out uint minimum, out uint maximum)
        {
            TimeCaps timeCaps = new TimeCaps(0, 0);
            uint result = timeGetDevCaps(out timeCaps, Marshal.SizeOf(timeCaps));
            if (result != 0) throw new Exception("timeGetDevCaps result=" + result);
            minimum = timeCaps.minimum;
            maximum = timeCaps.maximum;
        }
        
        private void TickHandler(uint id, uint msg, ref int userCtx, int rsv1, int rsv2)
        {
            DoElapsed();
        }

        /*[DllImport("Winmm.dll")]
        private static extern int timeGetTime();*/

        [DllImport("winmm.dll")]
        private static extern uint timeGetDevCaps(out TimeCaps timeCaps, int size);

        struct TimeCaps
        {
            public uint minimum;
            public uint maximum;

            public TimeCaps(uint minimum, uint maximum)
            {
                this.minimum = minimum;
                this.maximum = maximum;
            }
        }

        [DllImport("WinMM.dll", SetLastError = true)]
        private static extern uint timeSetEvent(int msDelay, int msResolution,
                    TimerEventHandler handler, ref int userCtx, int eventType);

        [DllImport("WinMM.dll", SetLastError = true)]
        static extern uint timeKillEvent(uint timerEventId);

        public delegate void TimerEventHandler(uint id, uint msg, ref int userCtx,
            int rsv1, int rsv2);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Stop();
                handler = null;
                if (disposing)
                {
                    
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~MultimediaTimer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Event members
        void DoElapsed()
        {
            OnElapsed(new EventArgs());
        }

        protected virtual void OnElapsed(EventArgs e)
        {
            if (Elapsed != null)
            {
                Elapsed(this, e);
            }
        }
        #endregion
    }
}
