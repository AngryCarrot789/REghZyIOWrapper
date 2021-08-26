using System.IO;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Arduino.Packets {
    [PacketImplementation]
    public class Packet3AnalogueRead : PacketACK {
        public int Value { get; }
        public int Pin { get; }

        // The server runs this to create a new fetch info request
        public static Packet3AnalogueRead ServerToHardwareGetInfo(int pin) {
            return new Packet3AnalogueRead(DestinationCode.ToClient, GetNextID<Packet0HardwareInfo>(), pin, -1);
        }

        // The client receives this as an acknowledgement that the server wants info
        public static Packet3AnalogueRead ServerToHardwareACK(int id, int pin) {
            return new Packet3AnalogueRead(DestinationCode.ClientACK, id, pin, -1);
        }

        // The client sends this to the server containing the data required
        public static Packet3AnalogueRead HardwareToServer(int id, int pin, int value) {
            return new Packet3AnalogueRead(DestinationCode.ToServer, id, pin, value);
        }

        static Packet3AnalogueRead() {
            RegisterPacket<Packet3AnalogueRead>(3, (meta, str) => {
                return null;
                // if (string.IsNullOrEmpty(str)) {
                //     throw new PacketCreationException("String value was null");
                // }
                // 
                // string[] split = str.Split('.');
                // if (split.Length == 0) {
                //     throw new PacketCreationException("Packet did not contain any dot separators");
                // }
                // 
                // DestinationCode destination = split[0].ParseEnum<DestinationCode>();
                // // this packet is received from a server
                // if (destination == DestinationCode.ToClient) {
                //     if (split.Length < 3) {
                //         throw new PacketCreationException("String did not contain 2 dot separators");
                //     }
                // 
                //     return ServerToHardwareACK(split[1].ParseInt(), (HardwareInfos)split[2].ParseInt());
                // }
                // // this packet is received from a client, so it holds very important info
                // else if (destination == DestinationCode.ClientACK || destination == DestinationCode.ToServer) {
                //     if (split.Length < 4) {
                //         throw new PacketCreationException("String did not contain 3 dot separators");
                //     }
                // 
                //     return HardwareToServer(split[1].ParseInt(), (HardwareInfos)split[2].ParseInt(), split[3]);
                // }
                // else {
                //     throw new PacketCreationException("Packet did not contain a know starting char (which states the source location)");
                // }
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
