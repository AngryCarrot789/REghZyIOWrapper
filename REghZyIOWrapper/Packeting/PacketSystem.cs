using System;
using System.Collections.Generic;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Packeting {
    /// <summary>
    /// A packet system is something that is capiable of transporting packets around 
    /// (sending new packets to hardware, sending received packets to listeners, etc)
    /// </summary>
    public abstract class PacketSystem {
        private List<IPacketListener> _listeners;

        public PacketSystem() {
            this._listeners = new List<IPacketListener>(8);
        }

        /// <summary>
        /// Registers a listener, allowing it to receive received packets by this packet system
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(IPacketListener listener) {
            if (this._listeners.Contains(listener)) {
                throw new Exception("This packet system already contained the given packet listener");
            }

            this._listeners.Add(listener);
        }

        /// <summary>
        /// Unregisters a listener, stopping it from receiving received packets by this packet system
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(IPacketListener listener) {
            if (this._listeners.Remove(listener)) {
                return;
            }

            throw new Exception("Couldn't unregister packet listener, because it was never registered!");
        }

        /// <summary>
        /// Called when a packet is received
        /// </summary>
        /// <param name="packet"></param>
        public void OnPacketReceived(Packet packet) {
            foreach (IPacketListener listener in this._listeners) {
                if (listener.OnReceivePacket(packet)) {
                    return;
                }
            }
        }

        /// <summary>
        /// Sends a packet to whatever handles packet communications
        /// </summary>
        /// <param name="packet"></param>
        public abstract void SendPacket(Packet packet);
    }
}
