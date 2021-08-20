using System;
using System.IO;
using System.IO.Ports;

namespace REghZyIOWrapper.Connections.Serial {
    /// <summary>
    /// A serial connection, that can send and receive data
    /// </summary>
    public class SerialDevice : IConnection, IDisposable {
        /// <summary>
        /// The serial port
        /// </summary>
        public SerialPort Port { get; }

        public string PortName { get => this.Port.PortName; }

        /// <summary>
        /// The thread-based reader, that constantly reads the serial port's input buffer
        /// </summary>
        private readonly SerialReader _reader;

        /// <summary>
        /// The serial port output stream
        /// </summary>
        public StreamWriter Output { get; private set; }

        /// <summary>
        /// The serial port input stream
        /// </summary>
        public StreamReader Input { get; private set; }

        /// <summary>
        /// Creates a new instance of a <see cref="SerialDevice"/>
        /// </summary>
        /// <param name="onLineReceived">Called when the <see cref="SerialReader"/> received a line of text</param>
        /// <param name="port">The port to connect and disconnect to and from</param>
        /// <param name="baudRate">The baud rate to operate the serial device at</param>
        public SerialDevice(Action<string> onLineReceived, string port, int baudRate = 9600) {
            if (onLineReceived == null) {
                throw new ArgumentNullException(nameof(onLineReceived), "Line received callback cannot be null");
            }
            if (port == null) {
                throw new ArgumentNullException(nameof(port), "Port cannot be null");
            }

            this.Port = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            this._reader = new SerialReader(this.Port, onLineReceived);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="SerialDevice"/>
        /// </summary>
        /// <param name="onLineReceived">Called when the <see cref="SerialReader"/> received a line of text</param>
        /// <param name="serialPort">The serial port object to use</param>
        public SerialDevice(Action<string> onLineReceived, SerialPort serialPort) {
            if (onLineReceived == null) {
                throw new ArgumentNullException(nameof(onLineReceived), "Line received callback cannot be null");
            }
            if (serialPort == null) {
                throw new ArgumentNullException(nameof(serialPort), "Serial Port cannot be null");
            }

            this.Port = serialPort;
            this._reader = new SerialReader(this.Port, onLineReceived);
            if (this.Port.IsOpen) {
                this.Output = new StreamWriter(this.Port.BaseStream);
                this.Output.AutoFlush = false;
                this.Input = new StreamReader(this.Port.BaseStream);
                this._reader.Enable();
            }
        }

        /// <summary>
        /// Connects to the serial device
        /// </summary>
        public void Connect() {
            this.Port.Open();
            this.Output = new StreamWriter(this.Port.BaseStream);
            this.Output.AutoFlush = false;
            this.Input = new StreamReader(this.Port.BaseStream);
            this._reader.Enable();
        }

        /// <summary>
        /// Disconnects from the serial device
        /// </summary>
        public void Disconnect() {
            this._reader.Disable();
            this.Input = null;
            this.Output = null;
            this.Port.Close();
        }

        /// <summary>
        /// Writes text to the output stream. 
        /// This will NOT immidiately write to the serial port, you must call <see cref="Flush"/> to do that
        /// </summary>
        public void Write(string text) {
            this.Output.Write(text);
        }

        /// <summary>
        /// Writes a character to the output stream. 
        /// This will NOT immidiately write to the serial port, you must call <see cref="Flush"/> to do that
        /// </summary>
        public void Write(char text) {
            this.Output.Write(text);
        }

        /// <summary>
        /// Writes text and then a new line character to the output stream. 
        /// This will NOT immidiately write to the serial port, you must call <see cref="Flush"/> to do that
        /// </summary>
        public void WriteLine(string line) {
            this.Output.Write(line);
            this.Output.Write('\n');
        }

        /// <summary>
        /// Writes a new line character to the output stream. 
        /// This will NOT immidiately write to the serial port, you must call <see cref="Flush"/> to do that
        /// </summary>
        public void WriteLine() {
            this.Output.WriteLine();
        }

        /// <summary>
        /// Flushes the output stream and writes the data to the serial port
        /// </summary>
        public void Flush() {
            this.Output.Flush();
        }

        public void Dispose() {
            this.Disconnect();
            this.Port.Dispose();
        }
    }
}
