using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERTS.Dashboard.Server
{
    public class InputControlServer : SimpleHTTPServer, IDisposable
    {
        const int CONNECTION_TIMEOUT = 500;
        //SimpleHTTPServer shs;

        CancellationTokenSource lastCancellationTokenSource;

        public event EventHandler<ReceivedInputEventArgs> ReceivedInput;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        public IPAddress ConnectedClient {
            get;
            private set;
        }
        DateTime lastInput;

        public InputControlServer() : base(8008)
        {
            //File.WriteAllText("index.htm", Properties.Resources.index);

            Assembly ass = Assembly.GetExecutingAssembly();
            foreach (String s in ass.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
        }

        protected override void Report(IPEndPoint remote, NameValueCollection inputData)
        {
            if (ConnectedClient == null)
            {
                Debug.WriteLine(String.Format("Client Connected."));
                DoClientConnected(remote.Address);
                ConnectedClient = remote.Address;
                lastInput = DateTime.Now;
            }
            else
            {
                if (!ConnectedClient.Equals(remote.Address))
                {
                    Debug.WriteLine(String.Format("Got data from bad client."));
                    return;
                }
            }
            if (Double.TryParse(inputData["lift"], out double lift) && Double.TryParse(inputData["pitch"], out double pitch) && Double.TryParse(inputData["roll"], out double roll) && Double.TryParse(inputData["yaw"], out double yaw))
            {
                if (lastCancellationTokenSource != null)
                {
                    lastCancellationTokenSource.Cancel();
                }

                DoReceivedInput(lift, pitch, roll, yaw);
                lastCancellationTokenSource = new CancellationTokenSource();
                Timeout(lastCancellationTokenSource.Token);
            }
        }

        async void Timeout(CancellationToken ct)
        {
            await Task.Delay(CONNECTION_TIMEOUT, ct).ContinueWith(tsk => { }); 

            if (!ct.IsCancellationRequested)
            {
                Debug.WriteLine(String.Format("Client Disconnected. (Timeout)"));
                DoClientDisconnected(ConnectedClient);
                ConnectedClient = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (IsAlive)
                    {
                        Stop();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InputControlServer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Event methods
        void DoReceivedInput(double lift, double pitch, double roll, double yaw)
        {
            OnReceivedInput(new ReceivedInputEventArgs(lift, pitch, roll, yaw));
        }

        protected virtual void OnReceivedInput(ReceivedInputEventArgs e)
        {
            ReceivedInput?.Invoke(this, e);
        }
        void DoClientConnected(IPAddress remote)
        {
            OnClientConnected(new ClientConnectedEventArgs(remote));
        }

        protected virtual void OnClientConnected(ClientConnectedEventArgs e)
        {
            if (ReceivedInput != null)
            {
                ClientConnected(this, e);
            }
        }

        void DoClientDisconnected(IPAddress remote)
        {
            OnClientDisconnected(new ClientDisconnectedEventArgs(remote));
        }

        protected virtual void OnClientDisconnected(ClientDisconnectedEventArgs e)
        {
            if (ReceivedInput != null)
            {
                ClientDisconnected(this, e);
            }
        }
        #endregion
    }
}
