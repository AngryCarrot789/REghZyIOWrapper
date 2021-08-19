using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Listeners.Filters {
    /// <summary>
    /// Accepts packets with a specific ID
    /// </summary>
    public class PacketIDFilter : PacketFilter {
        private readonly int _acceptedId;
        public int AcceptedID {
            get => this._acceptedId;
        }

        public PacketIDFilter(int acceptedId) {
            this._acceptedId = acceptedId;
        }

        public bool Accept(Packet packet) {
            return Packet.GetPacketID(packet) == this._acceptedId;
        }
    }
}
