using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace REghZyIOWrapper.Network.Connections.Serial {
    /// <summary>
    /// Uses a background thread to read incomming characters from a serial port
    /// </summary>
    public class SerialReader {
        private static int THREAD_COUNT = 0;
        private readonly Thread Thread;
        private volatile bool CanRun;

        public SerialPort Port { get; }

        private Action<string> _onLineReceived;
        // public Action<string> OnLineReceived {
        //     get => this._onLineReceived;
        //     set {
        //         if (value == null) {
        //             throw new NullReferenceException("Callback cannot be null");
        //         }
        // 
        //         this._onLineReceived = value;
        //     }
        // }

        public SerialReader(SerialPort port, Action<string> lineReceivedCallback) {
            if (port == null) {
                throw new NullReferenceException("Port cannot be null");
            }
            if (lineReceivedCallback == null) {
                throw new NullReferenceException("Callback cannot be null");
            }

            this.Port = port;
            this._onLineReceived = lineReceivedCallback;
            this.CanRun = false;
            this.Thread = new Thread(this.ReaderMain) {
                Name = "Serial Port Reader " + (THREAD_COUNT++),
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            this.Thread.Start();
        }

        public SerialReader(SerialPort port) {
            if (port == null) {
                throw new NullReferenceException("Port cannot be null");
            }

            this.Port = port;
            this.CanRun = false;
            this.Thread = new Thread(this.ReaderMain) {
                Name = "Serial Port Reader " + (THREAD_COUNT++),
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            this.Thread.Start();
        }

        public void Enable() {
            if (this.CanRun == true) {
                throw new Exception("Already enabled");
            }
            // if (this._onLineReceived == null) {
            //     throw new NullReferenceException("Callback cannot be null");
            // }

            this.CanRun = true;
        }

        public void Disable() {
            if (!this.CanRun) {
                throw new Exception("Already disabled");
            }

            this.CanRun = false;
        }

        private void ReaderMain() {
            SerialPort port = this.Port;
            StreamReader reader = new StreamReader(port.BaseStream);
            StringBuilder buffer = new StringBuilder(128);
            while (true) {
                if (this.CanRun) {
                    if (port.IsOpen) {
                        while (port.BytesToRead > 0) {
                            int read = reader.Read();
                            if (read == -1) {
                                continue;
                            }

                            char c = (char)read;
                            if (c == '\r') {
                                continue;
                            }
                            if (c == '\n') {
                                this._onLineReceived(buffer.ToString());
                                buffer.Clear();
                                continue;
                            }

                            buffer.Append(c);
                        }

                        Thread.Sleep(1);
                    }
                    else {
                        Thread.Sleep(1);
                    }
                }
                else {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
