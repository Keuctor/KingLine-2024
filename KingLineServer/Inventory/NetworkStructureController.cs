using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

public partial class Structure : INetSerializable
{
    public int Id { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(x);
        writer.Put(y);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        x = reader.GetFloat();
        y = reader.GetFloat();
    }
}


public class ReqStructures
{
}
public class ResStructures
{
    public Structure[] Structures { get; set; }
}


namespace KingLineServer.Inventory
{
    public class NetworkStructureController 
        : INetworkController
    {
        public static List<Structure> Structures = new List<Structure>();
        private void OnRequestStructures(ReqStructures request, NetPeer peer)
        {
            var packet = new ResStructures();
            packet.Structures = Structures.ToArray();
            PackageSender.SendPacket(peer, packet);
        }

        public void Subscribe(NetPacketProcessor processor)
        {
            processor.RegisterNestedType(() =>
            {
                return new Structure();
            });
            processor.SubscribeReusable<ReqStructures, NetPeer>(OnRequestStructures);
        }

        public void OnPeerDisconnected(NetPeer peer)
        {
        }

        public void OnPeerConnectionRequest(NetPeer peer, string username)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
        }

        public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
        {
        }
        public void OnExit()
        {

        }
        public void OnStart()
        {
            Structures.Add(new Structure()
            {
                Id = 0,
                x = 0,
                y = 0,
            });
        }
    }
}
