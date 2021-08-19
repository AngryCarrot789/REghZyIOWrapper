using REghZyIOWrapper.Network.Packeting;

namespace REghZyIOWrapper.Network.Hardware {
    public class ArduinoConnection : SerialPacketSystem {
        public ArduinoConnection(string port) : base(port) {

        }
    }
}
