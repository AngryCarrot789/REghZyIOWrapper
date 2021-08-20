using System;
using System.IO.Ports;
using REghZyIOWrapper.Connections;
using REghZyIOWrapper.Connections.Serial;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Packeting {
    /// <summary>
    /// An implementation of the <see cref="PacketSystem"/> (and <see cref="SpoolingPacketSystem"/>) that uses a <see cref="SerialDevice"/> to send and receive packets
    /// </summary>
    public class SerialPacketSystem : SpoolingPacketSystem, IConnection {
        /// <summary>
        /// The serial device used for reading and writing data
        /// </summary>
        public SerialDevice Device { get; }

        public SerialPacketSystem(string port) {
            this.Device = new SerialDevice(this.OnLineReceived, port);
        }

        public SerialPacketSystem(SerialPort port) {
            this.Device = new SerialDevice(this.OnLineReceived, port);
        }

        /// <summary>
        /// Connects to the serial device
        /// </summary>
        public void Connect() {
            this.Device.Connect();
        }

        /// <summary>
        /// Disconnects from the serial device
        /// </summary>
        public void Disconnect() {
            this.Device.Disconnect();
        }

        /// <summary>
        /// Immidiately sends the given packet. <see cref="SpoolingPacketSystem.QueuePacket(Packet)"/> is recommended to reduce waiting for packets to be written
        /// </summary>
        /// <param name="packet"></param>
        public override void SendPacket(Packet packet) {
            Packet.WritePacket(this.Device.Output, packet);
            this.Device.WriteLine();
            this.Device.Flush();
        }

        private void OnLineReceived(string line) {
            Packet packet;
            try {
                packet = Packet.CreatePacket(line);
            }
            catch(PacketCreationException e) {
                Console.WriteLine($"Failed to create packet: {(e.Message == null ? "(NO MESSAGE)" : e.Message)}");
                Console.WriteLine($"Raw packet data: {(e.PacketData == null ? "(PACKET DATA WAS NULL)" : e.PacketData)}");
                string stacktraceA = e.StackTrace;
                Console.WriteLine($"Stacktrace:\n{(stacktraceA == null ? "(UNAVAILABLE)" : stacktraceA)}");
                int count = 0;
                Exception inner = e.InnerException;
                while (inner != null && count < 20) {
                    Console.WriteLine($"Inner Exception: {(e.InnerException.Message == null ? "(NO MESSAGE)" : e.InnerException.Message)}");
                    string stacktraceB = e.InnerException.StackTrace;
                    Console.WriteLine($"Stacktrace:\n{(stacktraceB == null ? "(UNAVAILABLE)" : stacktraceB)}");
                    inner = inner.InnerException;
                    count++;
                }

                if (count == 20) {
                    Console.WriteLine("WARNING: INNER EXCEPTION LOOP!");
                }

                return;
            }
            catch (Exception e) {
                Console.WriteLine($"Unhandled exception trying to create a packet: {e.Message}");
                Console.WriteLine(e.StackTrace);
                return;
            }

            base.OnPacketReceived(packet);
        }
    }
}
