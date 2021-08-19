using System;

namespace REghZyIOWrapper.Network.Exceptions {
    public class PacketException : Exception {
        public PacketException(string message) : base(message) { }
    }
}
