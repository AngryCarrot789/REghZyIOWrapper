using System;

namespace REghZyIOWrapper.Exceptions {
    /// <summary>
    /// An exception that is thrown is a packet could not be created (e.g after being received from a device). 
    /// This could be caused by anything, e.g failed to parse an integer due to corrupt packet data
    /// </summary>
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
