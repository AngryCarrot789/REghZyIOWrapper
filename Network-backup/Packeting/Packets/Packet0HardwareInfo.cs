using System.IO;

namespace REghZyIOWrapper.Network.Packeting.Packets {
    public class Packet0HardwareInfo : Packet {
        public override int ID => 0;

        public Packet0HardwareInfo(int meta) : base(meta) { }

        public override void Write(TextWriter writer) {

        }

        public static Packet0HardwareInfo GetName() {
            return new Packet0HardwareInfo(1);
        }
    }
}
