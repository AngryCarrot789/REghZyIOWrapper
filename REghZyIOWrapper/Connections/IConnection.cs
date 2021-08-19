namespace REghZyIOWrapper.Connections {
    /// <summary>
    /// Something that can connect and disconnect, to and from something
    /// </summary>
    public interface IConnection {
        /// <summary>
        /// Connects to something
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the connection to something has failed</exception>
        void Connect();

        /// <summary>
        /// Disconnects from something
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the connection failed to disconnect from something</exception>
        void Disconnect();
    }
}
