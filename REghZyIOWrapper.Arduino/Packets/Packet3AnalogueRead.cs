using System.IO;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;
using static REghZyIOWrapper.Arduino.Packets.Packet0HardwareInfo;

namespace REghZyIOWrapper.Arduino.Packets {
    [PacketImplementation]
    public class Packet3AnalogueRead : PacketACK {
        public int Value { get; }
        public int Pin { get; }

        static Packet3AnalogueRead() {
            RegisterACKPacket<Packet3AnalogueRead>(3, (d, id, data) => {
                return new Packet3AnalogueRead(d, id, data.ParseInt(), -1);
            }, (d, id, data) => {
                return new Packet3AnalogueRead(d, id, data.GetWordAt(0, '.').ParseInt(), data.GetWordAt(1, '.').ParseInt());
            });
        }

        public Packet3AnalogueRead(DestinationCode destinationCode, int requestId, int pin, int value) : base(destinationCode, requestId) {
            this.Pin = pin;
            this.Value = value;
        }

        public override bool WriteToClient(TextWriter writer) {
            writer.Write($"{this.Pin}");
            return true;
        }

        public override bool WriteToServer(TextWriter writer) {
            writer.Write($"{this.Pin}.{this.Value}");
            return true;
        }
    }
}
