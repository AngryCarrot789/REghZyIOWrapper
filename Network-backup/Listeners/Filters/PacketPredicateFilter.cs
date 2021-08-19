using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REghZyIOWrapper.Network.Packets;

namespace REghZyIOWrapper.Network.Listeners.Filters {
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
