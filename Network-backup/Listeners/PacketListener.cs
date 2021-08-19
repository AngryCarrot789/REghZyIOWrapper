using System;
using REghZyIOWrapper.Network.Listeners.Filters;
using REghZyIOWrapper.Network.Packeting.Packets;

namespace REghZyIOWrapper.Network.Listeners {
    /// <summary>
    /// A class which holds a reference to a callback method that should be called
    /// if a packet received is accepted by the contained <see cref="PacketFilter"/>
    /// <para>
    /// By default, the <see cref="OnReceivePacket(Packet)"/> function returns <see langword="false"/>, so
    /// any other listener that listens to the same packets will receive those same packets
    /// </para>
    /// </summary>
    public class PacketListener {
        /// <summary>
        /// The filter that will be used to see if this <see cref="PacketListener"/> is allowed to be notified of
        /// a <see cref="Packet"/> received by the <see cref="SerialConnection"/>
        /// </summary>
        public PacketFilter Filter { get; }

        /// <summary>
        /// The callback function that is called if the <see cref="PacketFilter"/> allows the <see cref="Packet"/>
        /// </summary>
        public Action<Packet> OnPackedReceived { get; }

        private bool _cancelOnReceive;

        /// <summary>
        /// If <see langword="true"/>, when a packet is received by this listener (and obviously accepted by the filter), the same packet
        /// wont be sent to any more packet listeners.
        /// <para>
        /// If <see langword="false"/>, any other packet listeners will continue to listen to the same packet even after this one has received it 
        /// </para>
        /// <para>
        /// This is <see langword="false"/> by default
        /// </para>
        /// </summary>
        public bool CancelOnReceive { get => this._cancelOnReceive; }

        public PacketListener(PacketFilter filter, Action<Packet> onPacketReceived, bool cancelOnReceive = false) {
            if (filter == null) {
                throw new NullReferenceException("Filter cannot be null");
            }
            if (onPacketReceived == null) {
                throw new NullReferenceException("Packet received callback cannot be null");
            }

            this.Filter = filter;
            this.OnPackedReceived = onPacketReceived;
            this._cancelOnReceive = cancelOnReceive;
        }

        /// <summary>
        /// Checks if the contained <see cref="PacketFilter"/> will accept a <see cref="Packet"/>,
        /// and if so it calls the callback function (<see cref="OnPackedReceived"/>).
        /// </summary>
        /// <param name="packet">The packet to try and send to this listener</param>
        /// <returns>
        /// <see langword="true"/> if the packet is fully handled and shouldn't be processed or sent to any other listener.
        /// <see langword="false"/> if the same packet should be sent to other listeners
        /// </returns>
        public virtual bool OnReceivePacket(Packet packet) {
            if (this.Filter.Accept(packet)) {
                this.OnPackedReceived(packet);
                return _cancelOnReceive;
            }

            return false;
        }
    }
}
