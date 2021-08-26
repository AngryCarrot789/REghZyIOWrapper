﻿using System;
using REghZyIOWrapper.Packeting;

namespace REghZyIOWrapper.Arduino.Packets.ACK {
    /// <summary>
    /// A helper for getting hardware information within the network
    /// </summary>
    public class HardwareInfoACKHelper : ACKPacketHelper<Packet0HardwareInfo> {
        private readonly Func<Packet0HardwareInfo.HardwareInfos, string> GetInfoCallback;

        public HardwareInfoACKHelper(PacketSystem packetSystem, Func<Packet0HardwareInfo.HardwareInfos, string> getInfoCallback) : base(packetSystem) {
            this.GetInfoCallback = getInfoCallback;
        }

        public int SendRequest(Packet0HardwareInfo.HardwareInfos info) {
            Packet0HardwareInfo packet = Packet0HardwareInfo.ServerToHardwareGetInfo(info);
            this.SendPacket(packet);
            return packet.RequestID;
        }

        public override void OnProcessPacketToClientACK(Packet0HardwareInfo packet) {
            string info = this.GetInfoCallback(packet.Code);
            this.SendPacket(Packet0HardwareInfo.HardwareToServer(packet.RequestID, packet.Code, info));
        }

        public override void OnProcessPacketToServer(Packet0HardwareInfo packet) {
            // dont need to handle this packet, because it will be
            // listened to/waited for in the arduino device (GetHardwareNameAsync)
        }
    }
}
