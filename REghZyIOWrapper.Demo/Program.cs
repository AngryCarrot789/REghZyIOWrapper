using System;
using System.IO.Ports;
using REghZyIOWrapper.Arduino;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting;
using REghZyIOWrapper.Packeting.Packets;
using REghZyIOWrapper.Utils;

namespace REghZyIOWrapper.Demo {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Type a port name. Available ports: {string.Join(", ", SerialPort.GetPortNames())}");
            string port = Console.ReadLine();
            try {
                ArduinoDevice device = new ArduinoDevice(port);
                device.PacketSystem.RegisterListener(new GenericPacketListener<Packet8Chat>((packet) => Console.WriteLine("Incoming Chat: " + packet.Message)));
                device.Connect();
                Console.WriteLine($"Connected to {port}");
                Console.WriteLine($"Hardware name: {device.GetHardwareNameAsync().Result}");
                while (true) {
                    try {
                        string text = Console.ReadLine();
                        if (text == null || text.Length < 2) {
                            Console.WriteLine("Too short!");
                            continue;
                        }

                        int pin = text.Between("(", ")", 2).ParseInt();
                        if (pin < 3 || pin > 13) {
                            Console.WriteLine("Pin too low or high! Mst be between 3 and 13");
                            continue;
                        }

                        if (text.Substring(0, 2) == "dh") {
                            device.DigitalWrite(pin, true);
                        }
                        else if (text.Substring(0, 2) == "dl") {
                            device.DigitalWrite(pin, false);
                        }
                        else {
                            Console.WriteLine($"wut??? '{text.Substring(0, 2)}'. needs to be 'dh(pin)' for digital write high, or 'dl(pin)' for digital write low");
                        }
                    }
                    // literally effort cant be botherd to try parse integer XD
                    catch(Exception e) {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Failed to connect to {port}: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
