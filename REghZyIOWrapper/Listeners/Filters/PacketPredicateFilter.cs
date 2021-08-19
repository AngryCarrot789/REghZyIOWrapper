using System;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Listeners.Filters {
    public class PacketPredicateFilter : PacketFilter {
        private readonly Predicate<Packet> _predicate;

        public Predicate<Packet> Predicate {
            get => this._predicate;
        }

        public PacketPredicateFilter(Predicate<Packet> predicate) {
            if (predicate == null) {
                throw new NullReferenceException("Predicate cannot be null");
            }

            this._predicate = predicate;
        }

        public bool Accept(Packet packet) {
            return this._predicate(packet);
        }
    }
}
