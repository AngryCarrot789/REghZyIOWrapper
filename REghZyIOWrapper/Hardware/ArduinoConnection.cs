using REghZyIOWrapper.Packeting;

namespace REghZyIOWrapper.Hardware {
    public class ArduinoConnection : SerialPacketSystem {
        public ArduinoConnection(string port) : base(port) {

        }
    }
}
