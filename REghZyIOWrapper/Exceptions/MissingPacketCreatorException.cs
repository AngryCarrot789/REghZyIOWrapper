using System;

namespace REghZyIOWrapper.Exceptions {
    public class MissingPacketCreatorException : Exception {
        public int Id { get; }

        public MissingPacketCreatorException(int missingPacketId) :
            base($"A packet creator (for ID {missingPacketId}) was missing!") {
            this.Id = missingPacketId;
        }
    }
}
