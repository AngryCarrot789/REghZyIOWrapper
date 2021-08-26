using System;
using REghZyIOWrapper.Packeting;

namespace REghZyIOWrapper.Arduino.Packets.ACK {
    /// <summary>
    /// A helper class for reading analogue values from the hardware
    /// </summary>
    public class AnalogueReadACKHelper : ACKPacketHelper<Packet3AnalogueRead> {
        private readonly Func<int, int> GetAnalogueRead;

        public AnalogueReadACKHelper(PacketSystem packetSystem, Func<int, int> getAnalogeRead) : base(packetSystem) {
            this.GetAnalogueRead = getAnalogeRead;
        }

        public int SendRequest(int pin) {
            Packet3AnalogueRead packet = Packet3AnalogueRead.ServerToHardwareGetInfo(pin);
            this.SendPacket(packet);
            return packet.RequestID;
        }

        public override void OnProcessPacketToClientACK(Packet3AnalogueRead packet) {
            int value = this.GetAnalogueRead(packet.Pin);
            this.SendPacket(Packet3AnalogueRead.HardwareToServer(packet.RequestID, packet.Pin, value));
        }

        public override void OnProcessPacketToServer(Packet3AnalogueRead packet) {
            // dont need to handle this packet, because it will be
            // listened to/waited for in the arduino device (GetHardwareNameAsync)
        }
    }
}
