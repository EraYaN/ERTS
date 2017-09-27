using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace EraYaN.Serial
{
    public class SerialInterface : ISerial, IDisposable
    {
        const int blockLimit = 256;
        Action kickoffRead = null;
        string port;
        int baudrate;
        SerialPort serialPort;
        public string lastError = "";
        public event EventHandler<SerialDataEventArgs> SerialDataEvent;
        public bool IsOpen {
            get {
                return serialPort.IsOpen;
            }
        }
        public int BytesInTBuffer {
            get {
                return serialPort.BytesToWrite;
            }
        }
        public int BytesInRBuffer {
            get {
                return serialPort.BytesToRead;
            }
        }
        public SerialInterface(string _port, int _baudrate)
        {
            port = _port;
            baudrate = _baudrate;
            serialPort = new SerialPort
            {
                PortName = port,
                BaudRate = baudrate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.RequestToSendXOnXOff,
                ReceivedBytesThreshold = 1,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            //serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
            serialPort.PinChanged += serialPort_PinChanged;

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }
        }

        public int OpenPort()
        {
            try
            {
                Debug.WriteLine(String.Format("Opening port {0} at {1} baud.", port,baudrate), "SerialInterface");
                serialPort.Open();
                byte[] buffer = new byte[blockLimit];
                try
                {
                    kickoffRead = delegate
                    {
                        try
                        {
                            serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
                            {
                                try
                                {
                                    if (!serialPort.IsOpen)
                                        return;
                                    int actualLength = serialPort.BaseStream.EndRead(ar);
                                    byte[] received = new byte[actualLength];
                                    Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
                                    handleSerialData(received);

                                }
                                catch (IOException exc)
                                {
                                    //handleAppSerialError(exc);
                                    Debug.WriteLine(exc, "SerialInterface");
                                }
                                catch (InvalidOperationException exc)
                                {
                                    //port closed? race condition
                                    Debug.WriteLine(exc, "SerialInterface");
                                }

                                kickoffRead();
                            }, null);
                        }
                        catch (InvalidOperationException exc)
                        {
                            //port closed? cable disconnect
                            Debug.WriteLine(exc, "SerialInterface");
                        }
                    };
                    kickoffRead();
                }
                catch (InvalidOperationException exc)
                {
                    //port closed? race condition
                    Debug.WriteLine("Error closed?: " + exc.Message, "SerialInterface");
                }
                return 0;
            }
            catch (IOException e)
            {
                Debug.WriteLine("IOException: " + e.Message, "SerialInterface");
                lastError = e.Message;
                return -1;
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine("ArgumentException: " + e.Message, "SerialInterface");
                lastError = e.Message;
                return -2;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine("InvalidOperationException: " + e.Message, "SerialInterface");
                lastError = e.Message;
                return -3;
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine("UnauthorizedAccessException: " + e.Message, "SerialInterface");
                lastError = e.Message;
                return -4;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message, "SerialInterface");
                lastError = e.Message;
                return 1;
            }
        }
        public void SendString(string data)
        {
            serialPort.WriteLine(data);
        }
        public void SendByteArray(byte[] data)
        {
            try
            {
                serialPort.Write(data, 0, data.Length);
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("TX Timedout. Cable might be disconnected.", "SerialInterface");
            }
        }
        public void SendByte(byte data)
        {
            byte[] buf = { data };
            serialPort.Write(buf, 0, 1);
        }
        void serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            serialPort.Close();
            serialPort.Open();
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //MessageBox.Show();\
            Debug.WriteLine(String.Format("Serial Error {0}", e.EventType), "SerialInterface");
            //throw new NotImplementedException();
        }

        /*void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int input = serialPort.ReadByte();
            DataSerial((byte)input, e);           
        }*/

        void handleSerialData(byte[] data)
        {
            foreach (byte b in data)
            {
                DataSerial(b);
            }
        }

        void DataSerial(byte b)
        {
            OnSerialDataChanged(new SerialDataEventArgs(b));
        }

        protected virtual void OnSerialDataChanged(SerialDataEventArgs e)
        {
            if (SerialDataEvent != null)
            {
                SerialDataEvent(this, e);
            }
        }
    }
}
