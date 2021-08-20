using System.IO;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Arduino.Packets {
    public class Packet3AnalogueRead : Packet {
        public int Value { get; }

        public Packet3AnalogueRead(int value) {
            this.Value = value;
        }

        static Packet3AnalogueRead() {
            RegisterPacket(3, (meta, str) => new Packet3AnalogueRead(str.ParseInt()));
        }

        public override void Write(TextWriter writer) {

        }
    }
}
