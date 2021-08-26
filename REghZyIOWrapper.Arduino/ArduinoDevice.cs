using System;
using System.Threading.Tasks;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Arduino.Packets.ACK;
using REghZyIOWrapper.Connections;
using REghZyIOWrapper.Connections.Serial;
using REghZyIOWrapper.Packeting;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Arduino {
    public class ArduinoDevice : IConnection {
        public SerialPacketSystem PacketSystem { get; }
        public SerialDevice Device { get => this.PacketSystem.Device; }
        public HardwareInfoACKHelper HardwareNameHelper { get; }
        public AnalogueReadACKHelper AnalogueReadHelper { get; }

        public ArduinoDevice(string port) {
            this.PacketSystem = new SerialPacketSystem(port);
            this.HardwareNameHelper = new HardwareInfoACKHelper(this.PacketSystem, this.GetHardwareInfo);
            this.AnalogueReadHelper = new AnalogueReadACKHelper(this.PacketSystem, (pin) => { return 69; });
        }

        static ArduinoDevice() {
            // Packet.RunPacketCtor<Packet0HardwareInfo>();
            // Packet.RunPacketCtor<Packet1DigitalWrite>();
            // Packet.RunPacketCtor<Packet2PWMWrite>();
            // Packet.RunPacketCtor<Packet3AnalogueRead>();
            // Packet.RunPacketCtor<Packet8Chat>();
        }

        /// <summary>
        /// Sets the state of a digital pin
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="state"></param>
        public void DigitalWrite(int pin, bool state) {
            this.PacketSystem.SendPacket(new Packet1DigitalWrite(pin, state));
        }

        public void PWMWrite(int pin, int pwmValue) {
            this.PacketSystem.SendPacket(new Packet2PWMWrite(pin, pwmValue));
        }

        public async Task<int> AnalogueRead(int pin) {
            return await Task.Run(async () => {
                int id = this.AnalogueReadHelper.SendRequest(pin);
                Packet3AnalogueRead packet = await this.AnalogueReadHelper.ReceivePacketAsync(id);
                return packet.Value;
            });
        }

        public async Task<string> GetHardwareNameAsync() {
            return await Task.Run(async () => {
                int id = this.HardwareNameHelper.SendRequest(Packet0HardwareInfo.HardwareInfos.HardwareName);
                Packet0HardwareInfo packet = await this.HardwareNameHelper.ReceivePacketAsync(id);
                return packet.Information;
            });
        }

        public void Connect() {
            // this.PacketSystem.Enable();
            this.PacketSystem.Connect();
        }

        public void Disconnect() {
            // this.PacketSystem.Disable();
            this.PacketSystem.Disconnect();
        }

        public string GetHardwareInfo(Packet0HardwareInfo.HardwareInfos info) {
            switch (info) {
                case Packet0HardwareInfo.HardwareInfos.HardwareName:
                    return $"C# Software (Serial Port @ {this.Device.PortName})";
                case Packet0HardwareInfo.HardwareInfos.SerialPortName:
                    return this.Device.PortName;
                default:
                    return null;
            }
        }
    }
}
