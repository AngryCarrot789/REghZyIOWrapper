using System.IO;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Arduino.Packets {
    public class Packet8Chat : Packet {
        public string Message { get; }

        public Packet8Chat(string msg) {
            this.Message = msg;
        }

        static Packet8Chat() {
            // technically, you wouldn't need a packet creator for this packet,
            // because you only need to send the packet to the arduino, not receive it back
            RegisterPacket(8, (meta, msg) => {
                return new Packet8Chat(msg);
            });
        }

        public override void Write(TextWriter writer) {
            writer.Write(this.Message);
        }
    }
}
