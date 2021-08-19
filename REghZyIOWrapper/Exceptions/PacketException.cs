using System;

namespace REghZyIOWrapper.Exceptions {
    public class PacketException : Exception {
        public PacketException(string message) : base(message) { }
    }
}
