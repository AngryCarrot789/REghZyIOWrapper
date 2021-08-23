using System;
using System.IO.Ports;
using REghZyIOWrapper.Arduino;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Listeners;
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
                    string text = Console.ReadLine();
                    if (text == null || text.Length < 1) {
                        Console.WriteLine("Too short!");
                        continue;
                    }

                    int pin = -1;
                    string i = text.Between("(", ",");
                    if (i == null) {
                        pin = -1;
                    }

                    if (int.TryParse(i, out int integer)) {
                        pin = integer;
                    }

                    bool? state = null;
                    string st = text.Between(",", ")");
                    if (st == null) {
                        state = null;
                    }
                    else if (st.Trim().ToLower() == "high") {
                        state = true;
                    }
                    else if (st.Trim().ToLower() == "low") {
                        state = false;
                    }

                    if (pin < 3 || pin > 13) {
                        Console.WriteLine("Invalid pin! Must be between 3 and 13");
                        continue;
                    }

                    if (state == null) {
                        Console.WriteLine("State isn't valid!");
                        continue;
                    }

                    if (text.StartsWith("digitalWrite")) {
                        device.DigitalWrite(pin, state == true);
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Failed to connect to {port}: {e.Message}");
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
