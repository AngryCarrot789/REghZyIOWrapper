using System;

namespace REghZyIOWrapper.Exceptions {
    public class PacketCreationException : Exception {
        public PacketCreationException(string message) : base(message) {
        }
    }
}
