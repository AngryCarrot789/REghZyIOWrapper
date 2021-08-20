namespace REghZyIOWrapper.Packeting.Packets {
    /// <summary>
    /// The server creates <see cref="ToHardware"/> and sends it to the client. The client receives the raw packet data, and constructs 
    /// an acknowledgement (<see cref="HardwareACK"/>) packet which it processes internally (see <see cref="ArduinoDevice.HardwareInfoHelper"/>)
    /// and then creates a packet with <see cref="ToServer"/>, fills in the relevent information, and sends it back to the server. The server receives
    /// that and processes it (see <see cref="ArduinoDevice.HardwareInfoHelper"/>)
    /// </summary>
    public enum DestinationCode {
        /// <summary>
        /// This packet was sent from the server
        /// </summary>
        ToHardware = 1,

        /// <summary>
        /// This packet was sent from the server and received by the hardware
        /// </summary>
        HardwareACK = 2,

        /// <summary>
        /// This packet was sent from the hardware to the server
        /// </summary>
        ToServer = 3,
    }
}
