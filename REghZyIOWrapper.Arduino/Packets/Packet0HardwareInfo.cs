using System.IO;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Arduino.Packets {
    [PacketImplementation]
    public class Packet0HardwareInfo : PacketACK {

        // The first char specifies who sent the packet, aka the source; the server, or client
        // If it's S, the server wants info. If its C, the server has already requested info before, and the client is sending that info back
        // Server -> Hardware = "S.ID.TYPE"
        // Hardware -> Server = "C.ID.TYPE.DATA"

        public enum HardwareInfos {
            HardwareName = 1, // request the name of the hardware
            SerialPortName = 2  // The COM port the hardware is on
        }

        public HardwareInfos Code { get; }
        public string Information { get; }

        public Packet0HardwareInfo(DestinationCode destination, int requestId, HardwareInfos code, string returnInfo) : 
            base(destination, requestId) {
            this.Code = code;
            this.Information = returnInfo;
        }

        // // The server runs this to create a new fetch info request
        // public static Packet0HardwareInfo ServerToHardwareGetInfo(HardwareInfos info) {
        //     return new Packet0HardwareInfo(DestinationCode.ToClient, GetNextID<Packet0HardwareInfo>(), info, null);
        // }
        // 
        // // The client receives this as an acknowledgement that the server wants info
        // public static Packet0HardwareInfo ServerToHardwareACK(int id, HardwareInfos info) {
        //     return new Packet0HardwareInfo(DestinationCode.ClientACK, id, info, null);
        // }
        // 
        // // The client sends this to the server containing the data required
        // public static Packet0HardwareInfo HardwareToServer(int id, HardwareInfos info, string data) {
        //     return new Packet0HardwareInfo(DestinationCode.ToServer, id, info, data);
        // }

        static Packet0HardwareInfo() {
            RegisterACKPacket<Packet0HardwareInfo>(0, (d, id, data) => {
                return new Packet0HardwareInfo(d, id, data.ParseEnum<HardwareInfos>(), null);
            }, (d, id, data) => {
                return new Packet0HardwareInfo(d, id, data.GetWordAt(0, '.').ParseEnum<HardwareInfos>(), data.GetWordAt(1, '.'));
            });

            // RegisterPacket<Packet0HardwareInfo>(0, (meta, str) => {
            //     if (string.IsNullOrEmpty(str)) {
            //         throw new PacketCreationException("String value was null");
            //     }
            // 
            //     int dotA = str.IndexOf('.');
            //     if (dotA == -1) {
            //         throw new PacketCreationException("Packet did not contain any dot separators");
            //     }
            // 
            //     int dotB = str.IndexOf('.', dotA + 1);
            // 
            //     DestinationCode destination = str.JSubstring(0, dotA).ParseEnum<DestinationCode>();
            //     // this packet is received from a server
            //     if (destination == DestinationCode.ToClient) {
            //         if (dotB == -1) {
            //             throw new PacketCreationException("String did not contain 2 dot separators");
            //         }
            // 
            //         return ServerToHardwareACK(str.JSubstring(dotA + 1, dotB).ParseInt(), (HardwareInfos)str.JSubstring(dotB + 1).ParseInt());
            //     }
            //     // this packet is received from a client, so it holds very important info
            //     else if (destination == DestinationCode.ClientACK || destination == DestinationCode.ToServer) {
            //         int dotC = str.IndexOf('.', dotB + 1);
            //         if (dotC == -1) {
            //             throw new PacketCreationException("String did not contain 3 dot separators");
            //         }
            // 
            //         return HardwareToServer(str.JSubstring(dotA + 1, dotB).ParseInt(), (HardwareInfos)str.JSubstring(dotB + 1, dotC).ParseInt(), str.JSubstring(dotC + 1));
            //     }
            //     else {
            //         throw new PacketCreationException("Packet did not contain a know starting char (which states the source location)");
            //     }
            // });
        }

        public override bool WriteToClient(TextWriter writer) {
            writer.Write($"{(int)this.Code}");
            return true;
        }

        public override bool WriteToServer(TextWriter writer) {
            writer.Write($"{(int)this.Code}.{this.Information}");
            return true;
        }
    }
}
