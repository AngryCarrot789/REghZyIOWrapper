using System.Collections.Generic;
using System.Threading;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Packeting {
    /// <summary>
    /// Provides a method of queueing packets and automatically spooling them, using multithreading, to a <see cref="PacketSystem"/> implementation
    /// </summary>
    public abstract class SpoolingPacketSystem : PacketSystem {
        private static int SpoolersCount = 0;

        private readonly Thread WriteThread;
        private readonly Stack<Packet> Queue;

        private bool _canRunSpooler;
        public bool CanRunSpooler {
            get => this._canRunSpooler;
        }

        public SpoolingPacketSystem() {
            this.Queue = new Stack<Packet>();
            this._canRunSpooler = true;
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
            if (CanSpool()) {
                lock (this.Queue) {
                    SendPacket(this.Queue.Pop());
                }
            }
        }

        /// <summary>
        /// Checks if there's any packets to spool
        /// </summary>
        /// <returns></returns>
        public bool CanSpool() {
            return this.Queue.Count != 0;
        }

        /// <summary>
        /// Stops packets from being spooled
        /// </summary>
        public void Disable() {
            this._canRunSpooler = false;
        }

        /// <summary>
        /// Starts allowing packets to be spooled
        /// </summary>
        public void Enable() {
            this._canRunSpooler = true;
        }

        private void WriteMain() {
            while (true) {
                if (this._canRunSpooler) {
                    WriteNextPacket();
                    Thread.Sleep(1);
                }
            }
        }
    }
}
