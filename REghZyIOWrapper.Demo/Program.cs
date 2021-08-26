using System;
using System.IO.Ports;
using System.Threading;
using REghZyIOWrapper.Arduino;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Demo {
    class Program {
        static void Main(string[] args) {
            foreach (Type type in Packet.FindPacketImplementations()) {
                Console.WriteLine($"Found packet implementation '{type.Name}'");
                Packet.RunPacketCtor(type);
            }

            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                ArduinoDevice device = new ArduinoDevice(port);
                device.PacketSystem.RegisterListener(new GenericPacketListener<Packet8Chat>((packet) => Console.WriteLine("Incoming Chat: " + packet.Message)));
                device.PacketSystem.RegisterListener(new GenericPacketListener<Packet1DigitalWrite>((packet) => Console.WriteLine($"Digital Writing {packet.Pin} to {packet.State}")));
                device.Connect();
                Console.WriteLine($"Connected to {port}");
                Console.WriteLine($"Hardware name: {device.GetHardwareNameAsync().Result}");

                device.PacketSystem.SendPacket(new Packet1DigitalWrite(13, true));
                Thread.Sleep(1000);
                device.PacketSystem.SendPacket(new Packet1DigitalWrite(13, false));
            }
            catch (Exception exc) {
                Console.WriteLine($"Failed to connect to {port}: {exc.Message}");
                Console.WriteLine(exc.StackTrace);
            }

            Console.ReadLine();
        }
    }
}
