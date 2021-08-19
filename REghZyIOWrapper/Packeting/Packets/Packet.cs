using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using REghZyIOWrapper.Exceptions;

namespace REghZyIOWrapper.Packeting.Packets {
    /// <summary>
    /// <para>
    /// A packet is essentially a wrapper for data
    /// </para>
    /// <para>
    /// To write data, see <see cref="Write"/>
    /// </para>
    /// <para>
    /// Reading data is done automatically (see <see cref="RegisterCreator{T}"/> to register a creator)
    /// </para>
    /// </summary>
    public abstract class Packet {
        public const int TOTAL_PACKETS_POSSIBLE = 20;
        public const int PACKET_ID_MIN = 0;
        public const int PACKET_ID_MAX = 19;
        public const int PACKET_ID_LENGTH = 2;
        public const int PACKET_META_MIN = 0;
        public const int PACKET_META_MAX = 99;
        public const int PACKET_META_LENGTH = 2;
        public const int PACKET_HEADER_LENGTH = PACKET_ID_LENGTH + PACKET_META_LENGTH;

        /// <summary>
        /// An array of packet creators (taking the metadat and packet data (non-null)) and returns a packet instance
        /// <para>Index is the packet ID</para>
        /// </summary>
        private static readonly Func<int, string, Packet>[] PacketCreators;
        private static readonly Dictionary<int, Type> PacketIdToType;
        private static readonly Dictionary<Type, int> PacketTypeToId;

        static Packet() {
            PacketCreators = new Func<int, string, Packet>[TOTAL_PACKETS_POSSIBLE];
            PacketIdToType = new Dictionary<int, Type>();
            PacketTypeToId = new Dictionary<Type, int>();

            // RegisterCreator(0, (meta, data) => new Packet0Text(meta, data));
            // doesn't need to be registered because it's not a receivable packet, only sendable
            // software tells hardware to digitalWrite, not the other way around ;)
            // RegisterCreator(1, (meta, data) => new Packet1DigitalWrite(meta, data == "t"));
        }

        /// <summary>
        /// Forces all packet classes to be loaded
        /// </summary>
        public static void InitAssembly() {
            RunPacketCtor(typeof(Packet0HardwareInfo));
            RunPacketCtor(typeof(Packet1Text));
        }

        private static void RunPacketCtor(Type type) {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        /// <summary>
        /// The ID of this packet
        /// <para>
        /// Must NOT be below 0, or above 99 (100 possibly IDs)
        /// </para>
        /// </summary>
        // public abstract int ID { get; }

        /// <summary>
        /// Extra data for this packet (that is sent after the ID)
        /// <para>
        /// This cannot be below 0 or above 99 (100 possible metadata values)
        /// </para>
        /// </summary>
        public int MetaData { get; }

        /// <summary>
        /// Creates a packet (with the optional metadata)
        /// </summary>
        /// <param name="metaData"></param>
        protected Packet(int metaData = 0) {
            this.MetaData = metaData;
        }

        /// <summary>
        /// Writes the custom data in this packet to the given <see cref="TextWriter"/> (ID and Meta are written automatically, so this is optional)
        /// <para>
        /// This method must NOT write a new line character ('\n'), otherwise it could break everything. The '\r' will be ignored so writing it is pointless
        /// </para>
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Write(TextWriter writer);

        /// <summary>
        /// Registers a packet creator for a specific packet ID, with the given creator.
        /// Usually, you register creators in a packet class' static constructor
        /// </summary>
        /// <param name="id">The packet ID that the creator creates</param>
        /// <param name="creator">The function that creates the packet</param>
        /// <typeparam name="T">The type of packet to create</typeparam>
        public static void RegisterCreator<T>(int id, Func<int, string, T> creator) where T : Packet {
            Type type = typeof(T);
            if (type.IsAbstract) {
                throw new Exception("Packet type cannot be abstract");
            }

            if (PacketCreators[id] != null) {
                throw new Exception($"A packet creator (for the ID {id}) has already been registered");
            }

            PacketCreators[id] = creator;
            PacketIdToType[id] = type;
            PacketTypeToId[type] = id;
        }

        public static int GetPacketID<T>() where T : Packet {
            if (PacketTypeToId.TryGetValue(typeof(T), out int id)) {
                return id;
            }

            throw new Exception($"A packet (of type {typeof(T).Name}) was not registered");
        }

        public static int GetPacketID(Packet packet) {
            if (PacketTypeToId.TryGetValue(packet.GetType(), out int id)) {
                return id;
            }

            throw new Exception($"A packet (of type {packet.GetType().Name}) was not registered");
        }

        public static Type GetPacketType(int id) {
            if (PacketIdToType.TryGetValue(id, out Type type)) {
                return type;
            }

            throw new Exception($"The packet ID {id} was not registered");
        }

        /// <summary>
        /// Creates a packet from the given string (which contains a serialised packet)
        /// </summary>
        /// <param name="packetData">The full packet data (containing the ID, Meta and (optionally) custom packet data)</param>
        /// <returns>A packet instance (non-null)</returns>
        /// <exception cref="NullReferenceException">If the given packet data is null</exception>
        /// <exception cref="InvalidDataException">If the packet header data was corrupted (id or meta)</exception>
        /// <exception cref="MissingPacketCreatorException">If the packet creator for the specific Id was missing (aka null)</exception>
        /// <exception cref="PacketCreationException">Thrown if the packet couldn't be created (unknown metadata or corrupted packet data maybe)</exception>
        public static Packet CreatePacket(string packetData) {
            if (packetData == null) {
                throw new NullReferenceException("The data cannot be null!");
            }
            if (packetData.Length < PACKET_HEADER_LENGTH) {
                throw new InvalidDataException($"The data wasn't long enough, It must be {PACKET_HEADER_LENGTH} or above (it was {packetData.Length})");
            }

            string stringId = packetData.Substring(0, PACKET_ID_LENGTH);
            if (int.TryParse(stringId, out int id)) {
                string stringMeta = packetData.Substring(PACKET_ID_LENGTH, PACKET_META_LENGTH);
                if (int.TryParse(stringMeta, out int meta)) {
                    Func<int, string, Packet> creator = PacketCreators[id];
                    if (creator == null) {
                        throw new MissingPacketCreatorException(id);
                    }

                    return creator(meta, packetData.Substring(PACKET_HEADER_LENGTH));
                }

                throw new InvalidDataException($"The value ({stringMeta}) was not parsable as the packet's MetaData");
            }

            throw new InvalidDataException($"The value ({stringId}) was not parsable as the packet's ID");
        }

        /// <summary>
        /// Writes the given packet's ID, Meta to the given writer, and then calls the given packet's <see cref="Packet.Write(TextWriter)"/> method
        /// </summary>
        /// <param name="writer">The text writer to write the packet data to</param>
        /// <param name="packet">The packet that is to be written</param>
        /// <exception cref="InvalidDataException">If the packet's ID or Meta was invalid (below 0 or above 99)</exception>
        public static void WritePacket(TextWriter writer, Packet packet) {
            int id = GetPacketID(packet);
            if (id < PACKET_ID_MIN || id > PACKET_ID_MAX) {
                throw new InvalidDataException($"ID was not between {PACKET_META_MIN} and {PACKET_META_MAX}! It was {id}");
            }

            int meta = packet.MetaData;
            if (meta < PACKET_META_MIN || meta > PACKET_META_MAX) {
                throw new InvalidDataException($"Meta was not between {PACKET_META_MIN} and {PACKET_META_MAX}! It was {meta}");
            }

            writer.Write(StringFormatter.StretchFront(id.ToString(), PACKET_ID_LENGTH, '0'));
            writer.Write(StringFormatter.StretchFront(meta.ToString(), PACKET_META_LENGTH, '0'));
            packet.Write(writer);
        }
    }
}
