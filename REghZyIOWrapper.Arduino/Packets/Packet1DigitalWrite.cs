using System.IO;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Arduino.Packets {
    public class Packet1DigitalWrite : Packet {
        public int Pin { get; }
        public bool State { get; }

        public Packet1DigitalWrite(int pin, bool state) : base(pin) {
            this.Pin = pin;
            this.State = state;
        }

        static Packet1DigitalWrite() {
            // technically, you wouldn't need a packet creator for this packet,
            // because you only need to send the packet to the arduino, not receive it back
            RegisterPacket(1, (meta, stateStr) => {
                if (stateStr == null || stateStr.Length < 1) {
                    throw new PacketCreationException("String value was null or empty");
                }

                return new Packet1DigitalWrite(meta, stateStr == "H");
            });
        }

        public override void Write(TextWriter writer) {
            writer.Write(this.State ? 'H' : 'L');
        }
    }
}
