using System;

namespace REghZyIOWrapper.Network.Exceptions {
    public class PacketCreationException : Exception {
        public PacketCreationException(string message) : base(message) {
        }
    }
}
