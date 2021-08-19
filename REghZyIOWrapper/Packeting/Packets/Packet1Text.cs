using System;
using System.IO;

namespace REghZyIOWrapper.Packeting.Packets {
    public class Packet1Text : Packet {
        public string Text { get; }

        public Packet1Text(string text) {
            this.Text = text;
        }

        static Packet1Text() {
            RegisterCreator(1, (meta, str) => new Packet1Text(str));
        }

        public override void Write(TextWriter writer) {
            writer.Write(this.Text);
        }
    }
}
