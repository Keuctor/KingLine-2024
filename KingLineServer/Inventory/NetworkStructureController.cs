using LiteNetLib;
using LiteNetLib.Utils;

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

    public void OnUpdate(float deltaTime)
    {
    }
}