using System.IO;

namespace REghZyIOWrapper.Packeting.Packets {
    public class Packet0HardwareInfo : Packet {
        public enum Code {
            HardwareName = 1,
            HardwarePort = 2
        }

        public Code CodeInfo { get; }

        static Packet0HardwareInfo() {
            RegisterCreator(0, (meta, str) => new Packet0HardwareInfo((Code)meta));
        }

        public Packet0HardwareInfo(Code code) : base((int)code) {
            this.CodeInfo = code;
        }

        public override void Write(TextWriter writer) { }
    }
}
