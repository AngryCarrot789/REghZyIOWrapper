using System;
using System.IO;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace REghZyIOWrapper.Connections.Serial {
    /// <summary>
    /// Uses a background thread to read incomming characters from a serial port
    /// </summary>
    public class SerialReader : IDisposable {
        private static int THREAD_COUNT = 0;
        private readonly Thread Thread;
        private bool _threadCanLive = true;
        private volatile bool _canRun;

        /// <summary>
        /// The serial port reference
        /// </summary>
        public SerialPort Port { get; }

        private Action<string> _onLineReceived;

        private Dispatcher _creationThreadDispatcher;
        public Dispatcher CreationThreadDispatcher {
            get => this._creationThreadDispatcher;
        }

        public SerialReader(SerialPort port, Action<string> lineReceivedCallback) {
            if (port == null) {
                throw new NullReferenceException("Port cannot be null");
            }
            if (lineReceivedCallback == null) {
                throw new NullReferenceException("Callback cannot be null");
            }

            this._creationThreadDispatcher = Dispatcher.CurrentDispatcher;
            this.Port = port;
            this._onLineReceived = lineReceivedCallback;
            this._canRun = false;
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
            this._canRun = false;
            this.Thread = new Thread(this.ReaderMain) {
                Name = "Serial Port Reader " + (THREAD_COUNT++),
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            this.Thread.Start();
        }

        /// <summary>
        /// Enabled data to be received from the serial port
        /// </summary>
        public void Enable() {
            if (this._canRun == true) {
                throw new Exception("Already enabled");
            }
            // if (this._onLineReceived == null) {
            //     throw new NullReferenceException("Callback cannot be null");
            // }

            this._canRun = true;
        }

        /// <summary>
        /// Disabled data being received from the serial port
        /// </summary>
        public void Disable() {
            if (!this._canRun) {
                throw new Exception("Already disabled");
            }

            this._canRun = false;
        }

        private void OnLine(string line) {
            //this._creationThreadDispatcher.Invoke(()=> {
                this._onLineReceived(line);
            //});
        }

        private void ReaderMain() {
            SerialPort port = this.Port;
            StringBuilder buffer = new StringBuilder(128);
            while (this._threadCanLive) {
                if (this._canRun) {
                    if (port.IsOpen) {
                        int readable = port.BytesToRead;
                        if (readable > 0) {
                            while (readable > 0) {
                                if (this._canRun) {
                                    int read = port.ReadChar();
                                    readable = port.BytesToRead;
                                    if (read == -1) {
                                        continue;
                                    }

                                    char c = (char)read;
                                    if (c == '\r') {
                                        continue;
                                    }
                                    if (c == '\n') {
                                        OnLine(buffer.ToString());
                                        buffer.Clear();
                                        continue;
                                    }

                                    buffer.Append(c);
                                    continue;
                                }

                                readable = 0;
                            }
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

        public void Dispose() {
            this._threadCanLive = false;
            this._canRun = false;
            this.Thread.Join();
            this._onLineReceived = null;
        }
    }
}
