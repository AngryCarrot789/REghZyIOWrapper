using System;

namespace REghZyIOWrapper.Exceptions {
    public class PacketCreationException : Exception {
        public string PacketData { get; }

        public PacketCreationException(string message) : base(message) {

        }

        public PacketCreationException(string message, string packetData, Exception inner = null) : base(message, inner) {
            this.PacketData = packetData;
        }

        public PacketCreationException(string message, Exception inner = null) : base(message, inner) {

        }
    }
}
