using System.Collections.Generic;
using System.Threading;
using REghZyIOWrapper.Network.Packeting.Packets;

namespace REghZyIOWrapper.Network.Packeting {
    /// <summary>
    /// Provides a method of queueing packets and automatically spooling them, using multithreading, to a <see cref="PacketSystem"/> implementation
    /// </summary>
    public abstract class SpoolingPacketSystem : PacketSystem {
        private static int SpoolersCount = 0;

        private readonly Thread WriteThread;
        private readonly Stack<Packet> Queue;

        private bool _canSpool;
        public bool CanSpool {
            get => this._canSpool;
        }

        public SpoolingPacketSystem() {
            this.Queue = new Stack<Packet>();
            this._canSpool = true;
            this.WriteThread = new Thread(this.WriteMain);
            this.WriteThread.Name = $"REghZy Packet Spooler {SpoolersCount++}";
            this.WriteThread.Start();
        }

        /// <summary>
        /// Queues a packet to be spooled
        /// </summary>
        /// <param name="packet"></param>
        public void QueuePacket(Packet packet) {
            lock (this.Queue) {
                this.Queue.Push(packet);
            }
        }

        /// <summary>
        /// Forcefully writes the next queued packet
        /// </summary>
        public void WriteNextPacket() {
            if (CanWrite()) {
                lock (this.Queue) {
                    SendPacket(this.Queue.Pop());
                }
            }
        }

        public bool CanWrite() {
            return this.Queue.Count != 0;
        }

        /// <summary>
        /// Stops spooling packets
        /// </summary>
        public void Disable() {
            this._canSpool = false;
        }

        public void Enable() {
            this._canSpool = true;
        }

        private void WriteMain() {
            while (true) {
                if (this._canSpool) {
                    WriteNextPacket();
                    Thread.Sleep(1);
                }
            }
        }
    }
}
