﻿using System;
using System.Threading.Tasks;
using REghZyIOWrapper.Arduino.Packets;
using REghZyIOWrapper.Connections;
using REghZyIOWrapper.Connections.Serial;
using REghZyIOWrapper.Listeners;
using REghZyIOWrapper.Packeting;
using REghZyIOWrapper.Packeting.Packets;

namespace REghZyIOWrapper.Arduino {
    public class ArduinoDevice : IConnection {
        public SerialPacketSystem PacketSystem { get; }
        public SerialDevice Device { get => this.PacketSystem.Device; }
        public HardwareInfoHelper HardwareHelper { get; }

        public ArduinoDevice(string port) {
            this.PacketSystem = new SerialPacketSystem(port);
            this.HardwareHelper = new HardwareInfoHelper(this.PacketSystem, this.GetHardwareInfo);
        }

        static ArduinoDevice() {
            Packet.RunPacketCtor<Packet0HardwareInfo>();
            Packet.RunPacketCtor<Packet1DigitalWrite>();
            Packet.RunPacketCtor<Packet2PWMWrite>();
            Packet.RunPacketCtor<Packet3AnalogueRead>();
        }

        /// <summary>
        /// Sets the state of a digital pin
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="state"></param>
        public void DigitalWrite(int pin, bool state) {
            this.PacketSystem.QueuePacket(new Packet1DigitalWrite(pin, state));
        }

        public void PWMWrite(int pin, int pwmValue) {
            this.PacketSystem.QueuePacket(new Packet2PWMWrite(pin, pwmValue));
        }

        public async Task<string> GetHardwareNameAsync() {
            return await Task.Run(async () => {
                this.HardwareHelper.RequestInfo(Packet0HardwareInfo.HardwareInfos.HardwareName);
                return this.HardwareHelper.ReceivePacketAsync().Result.Information;
            });
        }

        public void Connect() {
            this.PacketSystem.Enable();
            this.PacketSystem.Connect();
        }

        public void Disconnect() {
            this.PacketSystem.Disable();
            this.PacketSystem.Disconnect();
        }

        public void OnHardwareInfoPacket(Packet0HardwareInfo packet) {
            // Console.WriteLine("Hardware Info Received: " + packet.Information);
        }

        public string GetHardwareInfo(Packet0HardwareInfo.HardwareInfos info) {
            switch (info) {
                case Packet0HardwareInfo.HardwareInfos.HardwareName:
                    return $"HRDWAR_NM={this.Device.PortName}";
                default:
                    return null;
            }
        }

        public class HardwareInfoHelper : ACKPacketHelper<Packet0HardwareInfo> {
            private readonly Func<Packet0HardwareInfo.HardwareInfos, string> GetInfoCallback;

            public HardwareInfoHelper(PacketSystem packetSystem, Func<Packet0HardwareInfo.HardwareInfos, string> getInfoCallback) : base(packetSystem) {
                this.GetInfoCallback = getInfoCallback;
            }

            public void RequestInfo(Packet0HardwareInfo.HardwareInfos info) {
                this.SendPacket(Packet0HardwareInfo.ServerToHardwareGetInfo(info));
            }

            public override void OnProcessPacketToClientACK(Packet0HardwareInfo packet) {
                string info = this.GetInfoCallback(packet.Code);
                this.SendPacket(Packet0HardwareInfo.HardwareToServer(packet.RequestID, packet.Code, info));
            }

            public override void OnProcessPacketToServer(Packet0HardwareInfo packet) {

            }
        }

        // could make this info an acknowledgable packet helper which contains some of this helpful stuff
        // public class HardwareInfoHelper {
        //     public Packet0HardwareInfo LastReceived;
        // 
        //     private readonly PacketSystem _packetSystem;
        //     private readonly Action<Packet0HardwareInfo> _onInfoReceived;
        //     private readonly Func<Packet0HardwareInfo.HardwareInfos, string> _getInfoCallback;
        // 
        //     public HardwareInfoHelper(PacketSystem packetSystem, Action<Packet0HardwareInfo> onInfoReceived, Func<Packet0HardwareInfo.HardwareInfos, string> getInfoCallback) {
        //         this._getInfoCallback = getInfoCallback;
        //         this._onInfoReceived = onInfoReceived;
        //         this._packetSystem = packetSystem;
        //         this._packetSystem.RegisterListener(new GenericPacketListener<Packet0HardwareInfo>(OnPacketReceived));
        //     }
        // 
        //     public void RequestInfo(Packet0HardwareInfo.HardwareInfos info) {
        //         this._packetSystem.SendPacket(Packet0HardwareInfo.ServerToHardwareGetInfo(info));
        //     }
        // 
        //     public void OnPacketReceived(Packet0HardwareInfo packet) {
        //         // acknowledgement
        //         if (packet.Destination == DestinationCode.HardwareACK) {
        //             string info = this._getInfoCallback(packet.Code);
        //             this._packetSystem.SendPacket(Packet0HardwareInfo.HardwareToServer(packet.RequestID, packet.Code, info));
        //         }
        //         // contains actual info
        //         else if (packet.Destination == DestinationCode.ToServer) {
        //             this.LastReceived = packet;
        //             this._onInfoReceived(packet);
        //         }
        //         // bug???
        //         else {
        //             throw new Exception("Received hardware info packet destination was not ACK or ToServer");
        //         }
        //     }
        // }
    }
}