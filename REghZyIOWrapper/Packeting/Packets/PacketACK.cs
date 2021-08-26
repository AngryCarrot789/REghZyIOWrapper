using System;
using System.Collections.Generic;
using System.IO;
using REghZyIOWrapper.Exceptions;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Packeting.Packets {
    /// <summary>
    /// A packet that supports sending, and receiving a value (obviously requiring asyncronous code)
    /// </summary>
    public abstract class PacketACK : Packet {
        public const int REQUEST_ID_MIN = 1;
        public const int REQUEST_ID_MAX = 99;
        public const int REQUEST_ID_LENGTH = 2;

        private static readonly Dictionary<Type, int> TypeToNextID = new Dictionary<Type, int>();

        /// <summary>
        /// The destination status of packet 
        /// <para>
        /// 1 if the packet was sent by the server (and we are the client)
        /// </para>
        /// <para>
        /// 2 if the packet was sent by the server (and we are the client) and is being processed by the client
        /// </para>
        /// <para>
        /// 3 if this packet was sent by the client, and is being processed by the server
        /// </para>
        /// </summary>
        public DestinationCode Destination { get; }

        /// <summary>
        /// The request ID for this type of packet, used to differentiate
        /// </summary>
        public int RequestID { get; }

        public PacketACK(DestinationCode destinationCode, int requestId) : base(requestId) {
            if (requestId < REQUEST_ID_MIN || requestId > REQUEST_ID_MAX) {
                throw new PacketCreationException($"Request ID ({requestId}) was not between {REQUEST_ID_MIN} and {REQUEST_ID_MAX}");
            }

            this.RequestID = requestId;
            this.Destination = destinationCode;
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
                if (nextId == REQUEST_ID_MAX) {
                    nextId = REQUEST_ID_MIN;
                }
                else {
                    nextId++;
                }
            }
            else {
                nextId = REQUEST_ID_MIN;
            }

            TypeToNextID[type] = nextId;
            return nextId;
        }

        public override void Write(TextWriter writer) {
            if (this.Destination == DestinationCode.ClientACK) {
                throw new Exception("Attempted to write ACK packet that was a client acknowledgement (Packet should've been recreated)");
            }

            if (this.Destination != DestinationCode.ToClient && this.Destination != DestinationCode.ToServer) {
                throw new Exception("Invalid destination code, it was not to the client or server");
            }

            writer.Write($"{(int)this.Destination}.{FormatRequestId()}.");
            if (this.Destination == DestinationCode.ToClient) {
                if (!WriteToClient(writer)) {
                    writer.Write('!');
                }
            }
            else if (this.Destination == DestinationCode.ToServer) {
                if (!WriteToServer(writer)) {
                    writer.Write('!');
                }
            }
            else {
                throw new Exception("Invalid destination code, it was not to the client or server");
            }
        }

        /// <summary>
        /// Returns a string containing the formatted request ID (usually with a 0 
        /// at the start if the request is below 9, forcing the length to be 2)
        /// </summary>
        /// <returns></returns>
        public string FormatRequestId() {
            return PacketFormatting.StretchFront(this.RequestID.ToString(), REQUEST_ID_LENGTH, '0');
        }

        /// <summary>
        /// Writes this packet data to the client. This will already write the destination code and request ID, and a "." on the end
        /// <para>
        /// If no data is required to be sent, return <see langword="false"/>. Otherwise, return true. 
        /// This will add a '!' char on the end, notifying to ignore the last param after the final '.'
        /// </para>
        /// </summary>
        /// <returns><see langword="true"/> if data was written, otherwise <see langword="false"/></returns>
        public abstract bool WriteToClient(TextWriter writer);

        /// <summary>
        /// Writes this packet data to the server. This will already write the destination code and request ID, and a "." on the end
        /// <para>
        /// If no data is required to be sent, return <see langword="false"/>. Otherwise, return true. 
        /// This will add a '!' char on the end, notifying to ignore the last param after the final '.'
        /// </para>
        /// </summary>
        /// <returns><see langword="true"/> if data was written, otherwise <see langword="false"/></returns>
        public abstract bool WriteToServer(TextWriter writer);
    }
}
