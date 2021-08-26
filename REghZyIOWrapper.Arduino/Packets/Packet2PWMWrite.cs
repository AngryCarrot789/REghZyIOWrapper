using System.IO;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Arduino.Packets {
    [PacketImplementation]
    public class Packet2PWMWrite : Packet {
        public int Pin { get; }
        public int PWM { get; }

        public Packet2PWMWrite(int pin, int pwm) : base(pin) {
            if (pwm < 0 || pwm > 255) {
                throw new PacketCreationException("PWM value must be between 0 and 255");
            }

            this.PWM = pwm;
            this.Pin = pin;
        }

        static Packet2PWMWrite() {
            // technically, you wouldn't need a packet creator for this packet,
            // because you only need to send the packet to the arduino, not receive it back
            RegisterPacket(2, (meta, stateStr) => {
                if (stateStr == null || stateStr.Length < 1) {
                    throw new PacketCreationException("String value was null or empty");
                }

                return new Packet2PWMWrite(meta, stateStr.ParseInt());
            });
        }

        public override void Write(TextWriter writer) {
            writer.Write(this.PWM);
        }
    }
}
