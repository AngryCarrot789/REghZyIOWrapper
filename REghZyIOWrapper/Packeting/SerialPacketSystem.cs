using System;
using REghZyIOWrapper.Connections;
using REghZyIOWrapper.Connections.Serial;
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
            catch(Exception e) {
                Console.WriteLine($"Failed to create packet: {e.Message}");
                return;
            }

            base.OnPacketReceived(packet);
        }
    }
}
