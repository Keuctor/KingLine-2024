using LiteNetLib;
using LiteNetLib.Utils;

    public class PackageSender
    {
        public static NetDataWriter _writer = new();
        public static NetPacketProcessor PacketProcessor;
        static bool debug = true;

        public static NetDataWriter WritePacket<T>(T packet) where T : class, new()
        {
            _writer.Reset();
            PacketProcessor.Write(_writer, packet);
            return _writer;
        }
        public static void SendPacket<T>(NetPeer peer, T packet) where T : class, new()
        {
            if (debug)
            {
                Cw.Log($"Sending packet to {peer.Id} {typeof(T)}", ConsoleColor.Gray);
            }
            peer.Send(WritePacket(packet), DeliveryMethod.ReliableOrdered);
        }
    }
