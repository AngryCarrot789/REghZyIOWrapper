using System;
using REghZyIOWrapper.Listeners.Filters;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Listeners {
    /// <summary>
    /// A packet listener that received certain packets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public class GenericPacketListener<T> : IPacketListener where T : Packet {
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
        public bool CancelOnReceive {
            get => this._cancelOnReceive;
        }

        private Type _acceptedType;
        public Type AcceptedType {
            get => this._acceptedType;
        }

        private Action<T> _onPacketReceived;
        /// <summary>
        /// The callback function that is called if the <see cref="PacketFilter"/> allows the <see cref="Packet"/>
        /// </summary>
        public Action<T> OnPacketReceived {
            get => this._onPacketReceived;
        }

        public GenericPacketListener(Action<T> onPacketReceived, bool cancelOnReceive = false) {
            this._acceptedType = typeof(T);
            this._cancelOnReceive = cancelOnReceive;
            this._onPacketReceived = onPacketReceived;
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
        public bool OnReceivePacket(Packet packet) {
            if (packet.GetType() == this._acceptedType) {
                this.OnPacketReceived((T)packet);
                return this._cancelOnReceive;
            }

            return false;
        }
    }
}
