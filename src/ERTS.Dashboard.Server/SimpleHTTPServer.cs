// MIT License - Copyright (c) 2016 Can Güney Aksakalli
// https://aksakalli.github.io/2014/02/24/simple-http-server-with-csparp.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Web;
using System.Collections.Specialized;

namespace ERTS.Dashboard.Server
{

    class SimpleHTTPServer
    {
        private readonly string[] _endPoints = {
        "/",
        "/style.css",
        "/script.js",
        "/input"
    };

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        
        {".css", "text/css"},        
        {".htm", "text/html"},
        {".html", "text/html"},
        {".js", "application/x-javascript"},        
        {".txt", "text/plain"}
        #endregion
    };
        private Thread _serverThread;
        private string _rootDirectory;
        private HttpListener _listener;
        private int _port;

        public int Port {
            get { return _port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="port">Port of the server.</param>
        public SimpleHTTPServer(int port)
        {
            this.Initialize(port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        public SimpleHTTPServer()
        {
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(port);
        }

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:" + _port.ToString() + "/");
            try
            {
                _listener.Start();
                while (true)
                {
                    try
                    {
                        HttpListenerContext context = _listener.GetContext();
                        Process(context);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    Debug.WriteLine("Please restart the application after you have granted permission to use the listener.", "HTTPSERVER");
                    Debug.WriteLine(String.Format("Use command `netsh http add urlacl url=http://+:{0}/ user={1}`.", _port, System.Security.Principal.WindowsIdentity.GetCurrent().Name), "HTTPSERVER");
                } else {

                    Debug.WriteLine(e.ToString());//netsh http add urlacl url = http://+:80/MyUri user=DOMAIN\user
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string endpoint = context.Request.Url.AbsolutePath;
            Debug.WriteLine(endpoint,"HTTPSERVER");
            if (!_endPoints.Contains(endpoint))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.OutputStream.Close();
                return;
            }

            switch (endpoint) {
                case "/":
                    Index(context.Response);
                    break;
                case "/style.css":
                    Style(context.Response);
                    break;
                case "/script.js":
                    Script(context.Response);
                    break;
                case "/input":
                    Input(context.Response,context.Request);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            context.Response.OutputStream.Close();
        }
        void Input(HttpListenerResponse response, HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
                string postData = reader.ReadToEnd();
                NameValueCollection inputData = HttpUtility.ParseQueryString(postData);

                if (inputData.AllKeys.Contains("lift") && inputData.AllKeys.Contains("pitch") && inputData.AllKeys.Contains("roll") && inputData.AllKeys.Contains("yaw"))
                {
                    Debug.WriteLine(String.Format("Got input: Lift {0:N2}; Pitch {1:N2}; Roll {2:N2}; Yaw {3:N2}", inputData["lift"], inputData["pitch"], inputData["roll"], inputData["yaw"]),"HTTPSERVER");
                    OK(response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }                             
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            }
        }

        void OK(HttpListenerResponse response)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("OK");
            response.ContentType = "text/plain";
            response.ContentLength64 = bytes.LongLength;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Flush();
        }

        void Index(HttpListenerResponse response)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Properties.Resources.index);
            response.ContentType = "text/html";
            response.ContentLength64 = bytes.LongLength;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Flush();
        }
        void Style(HttpListenerResponse response)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Properties.Resources.style);
            response.ContentType = "text/css";
            response.ContentLength64 = bytes.LongLength;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Flush();
        }
        void Script(HttpListenerResponse response)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Properties.Resources.script);
            response.ContentType = "text/javascript";
            response.ContentLength64 = bytes.LongLength;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Flush();
        }

        private void Initialize(int port)
        {
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }


    }
}