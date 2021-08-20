using System;
using System.Collections.Generic;
using System.IO;

namespace REghZyIOWrapper.Packeting.Packets {
    /// <summary>
    /// A packet that supports sending, and receiving a value (obviously requiring asyncronous code)
    /// </summary>
    public abstract class PacketACK : Packet {
        private static readonly Dictionary<Type, int> TypeToNextID = new Dictionary<Type, int>();

        /// <summary>
        /// 1 if the packet was received by the server
        /// <para>
        /// 2 if this packet was received by the client, but it contains the actual requested info
        /// </para>
        /// <para>
        /// 3 if this packet was received by the client, but it was only an acknowledgement (no info)
        /// </para>
        /// </summary>
        public DestinationCode Destination { get; }

        /// <summary>
        /// The request ID for this type of packet, used to differentiate
        /// </summary>
        public int RequestID { get; }

        public PacketACK(DestinationCode destinationCode, int requestId) : base(requestId) {
            this.Destination = destinationCode;
            this.RequestID = requestId;
        }

        /// <summary>
        /// Gets the next ACK ID for a specific packet type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetNextID<T>() where T : PacketACK {
            Type type = typeof(T);
            int nextId;
            if (TypeToNextID.TryGetValue(type, out nextId)) {
                if (nextId == PACKET_META_MAX) {
                    nextId = PACKET_META_MIN;
                }
                else {
                    nextId++;
                }
            }
            else {
                nextId = PACKET_META_MIN;
            }

            TypeToNextID[type] = nextId;
            return nextId;
        }

        public override void Write(TextWriter writer) {
            if (this.Destination == DestinationCode.ToHardware) {
                WriteToHardware(writer);
            }
            else if (this.Destination == DestinationCode.ToServer) {
                WriteToServer(writer);
            }
        }

        public abstract void WriteToHardware(TextWriter writer);
        public abstract void WriteToServer(TextWriter writer);
    }
}
