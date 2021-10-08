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
            // Locate all of the packet classes
            foreach (Type type in Packet.FindPacketImplementations()) {
                Console.WriteLine($"Found packet implementation '{type.Name}'");
                // Force thr CLR to run their static constructor (which registers the packet)
                Packet.RunPacketCtor(type);
            }

            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                ArduinoDevice device = new ArduinoDevice(port);
                // Register a few packet listeners
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
                Thread.Sleep(1000);
                device.DigitalWrite(12, true);
                Thread.Sleep(1000);
                device.DigitalWrite(11, true);
                Thread.Sleep(1000);
                device.DigitalWrite(13, false);
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
