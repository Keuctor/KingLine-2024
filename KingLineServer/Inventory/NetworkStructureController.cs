using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;

public class MapStructure
{
    public Structure Structure { get; set; }
    public int TroopCount { get; set; }
    public int MaxTroopCount { get; set; }
}
public class NetworkStructureController
    : INetworkController
{
    static Random random = new Random();

    public static Dictionary<int, MapStructure> Structures = new Dictionary<int, MapStructure>();

    private static Structure[] structures = new Structure[0];
    public static Structure[] GetStructures()
    {
        if (structures.Length == 0)
        {
            structures = new Structure[Structures.Count];
            for (int i = 0; i < Structures.Count; i++)
            {
                structures[i] = Structures[i].Structure;
            }
        }
        return structures;
    }
    private void OnRequestStructures(ReqStructures request, NetPeer peer)
    {
        var packet = new ResStructures();
        packet.Structures = GetStructures();
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
        processor.SubscribeReusable<ReqBuyVolunteers, NetPeer>(OnRequestBuyVolunteers);
    }



    private void OnRequestBuyVolunteers(ReqBuyVolunteers request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var troop = TroopRegistry.Troops[request.Id];

        if (player.Currency >= troop.Price * request.Count)
        {
            for (int i = 0; i < Structures.Count; i++)
            {
                var structure = Structures[i];
                if (structure.Structure.Id == request.StructureId)
                {
                    if (structure.TroopCount >= request.Count)
                    {
                        structure.TroopCount -= request.Count;
                        player.Currency -= troop.Price * request.Count;

                        PackageSender.SendPacket(peer, new ResPlayerCurrency()
                        {
                            NewCurrency = player.Currency
                        }, DeliveryMethod.ReliableUnordered);


                        NetworkPlayerTeamController.AddMember(player.Token, request.Id,
                            request.Count);

                        PackageSender.SendPacket(peer, new ResUpdatePlayerTeam()
                        {
                            Team = new Team()
                            {
                                Id = player.Id,
                                Members = NetworkPlayerTeamController.PlayerTeams[player.Token]
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine(">>Not enough troops in place to buy");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Player does not have enough money");
        }
    }

    private void OnRequestVolunteers(ReqVolunteers request, NetPeer peer)
    {
        for (int i = 0; i < Structures.Count; i++)
        {
            var structure = Structures[i];
            if (structure.Structure.Id == request.StructureId)
            {
                var response = new ResVolunteers()
                {
                    Count = (short)structure.TroopCount,
                    TroopId = (int)TroopType.PEASANT,
                    StructureId = request.StructureId
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
        Structures.Add(0, new MapStructure()
        {
            Structure = new Structure()
            {
                Id = 0,
                x = 0,
                y = 0,
            },
            TroopCount = 3,
            MaxTroopCount = 5
        });
    }

    int troopSpawnTimer = 30 * 60;
    int timer = 0;
    public void OnUpdate(float deltaTime)
    {
        timer++;
        if (timer < troopSpawnTimer) return;
        timer = 0;
        Console.WriteLine("Troops updated");
        foreach (var structure in Structures)
        {
            var mapStructure = structure.Value;
            if (random.NextDouble() >= 0.35)
            {
                if (mapStructure.TroopCount < mapStructure.MaxTroopCount)
                {
                    structure.Value.TroopCount += 1;
                }
            }
            else
            {
                if (mapStructure.TroopCount > 0)
                {
                    structure.Value.TroopCount -= 1;
                }
            }
        }
    }
}