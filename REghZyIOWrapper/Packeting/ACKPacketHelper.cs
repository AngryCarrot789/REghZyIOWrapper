﻿using System;
using System.Threading.Tasks;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Packeting {
    /// <summary>
    /// A helper class for managing ACK packets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ACKPacketHelper<T> where T : PacketACK {
        private T _lastReceivedPacket;

        /// <summary>
        /// The latest packet that was received
        /// </summary>
        public T LastReceivedPacket {
            get => this._lastReceivedPacket;
        }

        private readonly PacketSystem _packetSystem;
        public PacketSystem PacketSystem {
            get => this._packetSystem;
        }

        protected ACKPacketHelper(PacketSystem packetSystem) {
            if (packetSystem == null) {
                throw new ArgumentNullException(nameof(packetSystem), "Packet system cannot be null");
            }

            this._packetSystem = packetSystem;
            this._packetSystem.RegisterListener(new GenericPacketListener<T>(OnPacketReceived));
        }

        private void OnPacketReceived(T packet) {
            // acknowledgement
            if (packet.Destination == DestinationCode.HardwareACK) {
                OnProcessPacketToClientACK(packet);
            }
            // contains actual info
            else if (packet.Destination == DestinationCode.ToServer) {
                this._lastReceivedPacket = packet;
                OnProcessPacketToServer(packet);
            }
            // bug???
            else {
                throw new Exception("Received hardware info packet destination was not ACK or ToServer");
            }
        }

        /// <summary>
        /// Waits until the last received packet is no longer null (meaning a new ACK packet has arrived), and then returns it as the task's result
        /// <para>
        /// This will wait forever until the packet has arrived
        /// </para>
        /// </summary>
        public async Task<T> ReceivePacketAsync() {
            while(this._lastReceivedPacket == null) {
                await Task.Delay(1).ConfigureAwait(true);
            }

            return this._lastReceivedPacket;
        }

        public void SendPacket(T packet) {
            this._packetSystem.SendPacket(packet);
        }

        /// <summary>
        /// Called when the client receives a packet from the server, aka the mid-way between getting 
        /// and receiving data (that is, if the ACK packet is used for that)
        /// </summary>
        public abstract void OnProcessPacketToClientACK(T packet);

        /// <summary>
        /// Called when the server receives a packet from the client
        /// </summary>
        public abstract void OnProcessPacketToServer(T packet);
    }
}