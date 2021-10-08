using System;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Threading;
using REghZyIOWrapper.Arduino;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Demo {
    class Program {
        static void Main(string[] args) {
            Packet.RunPacketCtor<Packet>();
            foreach (Type type in Packet.FindPacketImplementations()) {
                Console.WriteLine($"Found packet implementation '{type.Name}'");
                Packet.RunPacketCtor(type);
            }

            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                ArduinoDevice device = new ArduinoDevice(port);
                device.PacketSystem.RegisterListener(new GenericPacketListener<Packet8Chat>((packet) => {
                    Console.WriteLine("Incoming Chat: " + packet.Message);
                }));
                device.PacketSystem.RegisterListener(new GenericPacketListener<Packet1DigitalWrite>((packet) => {
                    Console.WriteLine($"Digital Writing {packet.Pin} to {packet.State}");
                }));

                device.Connect();
                Console.WriteLine($"Connected to {port}");
                Console.WriteLine($"Hardware name: {device.GetHardwareNameAsync().Result}");
                Console.WriteLine($"e");

                Console.Write("e");
                device.DigitalWrite(13, true);
                device.DigitalWrite(12, true);
                device.DigitalWrite(11, true);
                device.DigitalWrite(13, false);
                device.DigitalWrite(10, true);
                device.DigitalWrite(12, false);
                device.DigitalWrite(11, false);
                device.DigitalWrite(10, false);
                Console.Write("e");
            }
            catch (Exception exc) {
                Console.WriteLine($"Failed to connect to {port}: {exc.Message}");
                Console.WriteLine(exc.StackTrace);
            }

            Console.ReadLine();
        }
    }
}
