using System;
using System.IO.Ports;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Demo {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                Packet.InitAssembly();
                SerialPacketSystem system = new SerialPacketSystem(port);
                system.RegisterListener(new GenericPacketListener<Packet0HardwareInfo>((packet) => { Console.WriteLine($"Hardware Info: {Enum.GetName(packet.CodeInfo.GetType(), packet.CodeInfo)}"); }, true));
                system.RegisterListener(new GenericPacketListener<Packet1Text>((packet) => { Console.WriteLine($"Text Received: {packet.Text}"); }, true));
                system.Enable();
                system.Connect();
                Console.WriteLine($"Connected to {port}");
                while (true) {
                    string text = Console.ReadLine();
                    system.SendPacket(new Packet1Text(text));
                    system.SendPacket(new Packet0HardwareInfo(Packet0HardwareInfo.Code.HardwareName));
                    system.SendPacket(new Packet0HardwareInfo(Packet0HardwareInfo.Code.HardwarePort));
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Failed to connect to {port}: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
