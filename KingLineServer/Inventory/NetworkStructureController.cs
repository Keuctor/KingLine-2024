using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkStructureController
    : INetworkController
{
    static Random random = new Random();

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
        processor.SubscribeReusable<ReqVolunteers, NetPeer>(OnRequestVolunteers);
    }

    private void OnRequestVolunteers(ReqVolunteers request, NetPeer peer)
    {
        for (int i = 0; i < Structures.Count; i++) {
            var structure = Structures[i];
            if (structure.Id == request.StructureId) {
                var rand = random.Next(2,6);
                var response = new ResVolunteers() {
                    Count = 0,
                    TroopId = (int)TroopType.PEASANT
                };
                PackageSender.SendPacket(peer, response);
            }
        }
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