using System;
using System.IO;
using System.IO.Ports;
using REghZyIOWrapper.Connections;
using REghZyIOWrapper.Connections.Serial;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Packeting {
    /// <summary>
    /// An implementation of the <see cref="PacketSystem"/> that uses a <see cref="SerialDevice"/> to send and receive packets
    /// </summary>
    public class SerialPacketSystem : PacketSystem, IConnection {
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
        /// Immidiately sends the given packet to the serial device
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
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine($"Failed to create packet: {e.Message ?? "(NO MESSAGE)"}");
                Console.WriteLine($"Raw packet data: {e.PacketData ?? "(PACKET DATA WAS NULL)"}");
                Console.WriteLine($"Stacktrace:");
                Console.WriteLine(e.StackTrace ?? "(UNAVAILABLE)");
                int count = 0;
                Exception inner = e.InnerException;
                while (inner != null && count < 20) {
                    Console.WriteLine($"Inner Exception: {e.InnerException.Message ?? "(NO MESSAGE)"}");
                    string stacktraceB = e.InnerException.StackTrace;
                    Console.WriteLine($"Stacktrace:\n{stacktraceB ?? "(UNAVAILABLE)"}");
                    inner = inner.InnerException;
                    count++;
                }

                if (count == 20) {
                    Console.WriteLine("WARNING: INNER EXCEPTION LOOP!");
                }

                Console.WriteLine("------------------------------------------------------------");
                return;
            }
            catch (InvalidDataException e) {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine($"Failed to create a packet due to a corrupt packet header: {e.Message ?? "(UNAVAILABLE)"}");
                Console.WriteLine("Data: " + line);
                Console.WriteLine(e.StackTrace ?? "(UNAVAILABLE)");
                Console.WriteLine("------------------------------------------------------------");
                return;
            }
            catch (Exception e) {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine($"Unhandled exception trying to create a packet: {e.Message ?? "(UNAVAILABLE)"}");
                Console.WriteLine("Data: " + line);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("------------------------------------------------------------");
                return;
            }

            base.OnPacketReceived(packet);
        }
    }
}
