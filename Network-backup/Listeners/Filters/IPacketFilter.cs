using REghZyIOWrapper.Network.Packeting.Packets;

namespace REghZyIOWrapper.Network.Listeners.Filters {
    /// <summary>
    /// Used for checking if a packet is acceptable
    /// </summary>
    public interface PacketFilter {
        /// <summary>
        /// Returns <see langword="true"/> if the given <see cref="Packet"/> is allowed by this <see cref="PacketFilter"/>
        /// </summary>
        /// <param name="packet">The packet to check</param>
        /// <returns></returns>
        bool Accept(Packet packet);
    }
}
