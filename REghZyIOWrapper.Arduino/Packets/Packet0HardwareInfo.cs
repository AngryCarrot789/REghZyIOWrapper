using System.IO;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Arduino.Packets {
    public sealed class Packet0HardwareInfo : PacketACK {

        // The first char specifies who sent the packet, aka the source; the server, or client
        // If it's S, the server wants info. If its C, the server has already requested info before, and the client is sending that info back
        // Server -> Hardware = "S.ID.TYPE"
        // Hardware -> Server = "C.ID.TYPE.DATA"

        public enum HardwareInfos {
            HardwareName = 1 // request the name of the hardware
        }

        public HardwareInfos Code { get; }
        public string Information { get; }

        public Packet0HardwareInfo(DestinationCode destination, int requestId, HardwareInfos code, string returnInfo) : 
            base(destination, requestId) {
            this.Code = code;
            this.Information = returnInfo;
        }

        // The server runs this to create a new fetch info request
        public static Packet0HardwareInfo ServerToHardwareGetInfo(HardwareInfos info) {
            return new Packet0HardwareInfo(DestinationCode.ToHardware, GetNextID<Packet0HardwareInfo>(), info, null);
        }

        // The client receives this as an acknowledgement that the server wants info
        public static Packet0HardwareInfo ServerToHardwareACK(int id, HardwareInfos info) {
            return new Packet0HardwareInfo(DestinationCode.HardwareACK, id, info, null);
        }

        // The client sends this to the server containing the data required
        public static Packet0HardwareInfo HardwareToServer(int id, HardwareInfos info, string data) {
            return new Packet0HardwareInfo(DestinationCode.ToServer, id, info, data);
        }

        static Packet0HardwareInfo() {
            RegisterPacket<Packet0HardwareInfo>(0, (meta, str) => {
                if (string.IsNullOrEmpty(str)) {
                    throw new PacketCreationException("String value was null");
                }

                string[] split = str.Split('.');
                if (split.Length == 0) {
                    throw new PacketCreationException("Packet did not contain any dot separators");
                }

                DestinationCode destination = split[0].ParseEnum<DestinationCode>();
                // this packet is received from a server
                if (destination == DestinationCode.ToHardware) {
                    if (split.Length < 3) {
                        throw new PacketCreationException("String did not contain 2 dot separators");
                    }

                    return ServerToHardwareACK(split[1].ParseInt(), (HardwareInfos)split[2].ParseInt());
                }
                // this packet is received from a client, so it holds very important info
                else if (destination == DestinationCode.HardwareACK || destination == DestinationCode.ToServer) {
                    if (split.Length < 4) {
                        throw new PacketCreationException("String did not contain 3 dot separators");
                    }

                    return HardwareToServer(split[1].ParseInt(), (HardwareInfos)split[2].ParseInt(), split[3]);
                }
                else {
                    throw new PacketCreationException("Packet did not contain a know starting char (which states the source location)");
                }
            });
        }

        public override void WriteToHardware(TextWriter writer) {
            writer.Write($"{(int)this.Destination}.{PacketFormatting.StretchFront(this.RequestID.ToString(), 2, '0')}.{(int)this.Code}");
        }

        public override void WriteToServer(TextWriter writer) {
            writer.Write($"{(int)this.Destination}.{PacketFormatting.StretchFront(this.RequestID.ToString(), 2, '0')}.{(int)this.Code}.{this.Information}");
        }
    }
}
