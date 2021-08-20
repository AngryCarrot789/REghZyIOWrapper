using System;
using System.IO.Ports;
using REghZyIOWrapper.Arduino;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Demo {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                ArduinoDevice device = new ArduinoDevice(port);
                device.PacketSystem.RegisterListener(
                    new GenericPacketListener<Packet0HardwareInfo>(
                        (packet) => {
                            Console.WriteLine(
                                $"Debug Hardware Packet: {Enum.GetName(packet.Code.GetType(), packet.Code)}, " +
                                $"Data: {(packet.Information == null ? "NULL" : packet.Information)}, " +
                                $"ID: {packet.RequestID}, " +
                                $"Destination: {packet.Destination}");
                        }));
                device.Connect();
                Console.WriteLine($"Connected to {port}");
                while (true) {
                    string text = Console.ReadLine();
                    string name = device.GetHardwareNameAsync().Result;
                    Console.WriteLine($"Hardware name: {name}");
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Failed to connect to {port}: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
