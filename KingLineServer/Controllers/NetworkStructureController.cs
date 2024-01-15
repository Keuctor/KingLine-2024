using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Numerics;

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
        processor.SubscribeReusable<ReqStructureInventory, NetPeer>(OnRequestStructureInventory);
    }

    private void OnRequestStructureInventory(ReqStructureInventory request, NetPeer peer)
    {
        var targetStructure = Structures[request.StructureId];
        if (targetStructure == null)
        {
            return;
        }
        var inv = NetworkInventoryController.Inventories[targetStructure.Structure.InventoryId];
        PackageSender.SendPacket(peer, new ResStructureInventory()
        {
            Items = inv.Items,
            InventoryId = inv.Id,
            StructureId = request.StructureId,
        }, DeliveryMethod.ReliableUnordered);
    }

    private void OnRequestBuyVolunteers(ReqBuyVolunteers request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var troop = TroopRegistry.Troops[request.Id];

        if (player.Gold >= troop.Price * request.Count)
        {
            for (int i = 0; i < Structures.Count; i++)
            {
                var structure = Structures[i];
                if (structure.Structure.Id == request.StructureId)
                {
                    if (structure.TroopCount >= request.Count)
                    {
                        structure.TroopCount -= request.Count;
                        player.Gold -= troop.Price * request.Count;

                        PackageSender.SendPacket(peer, new ResPlayerCurrency()
                        {
                            NewCurrency = player.Gold
                        }, DeliveryMethod.ReliableUnordered);


                        var token = KingLine.GetPlayerToken(peer.Id);
                        NetworkPlayerTeamController.AddMember(token, request.Id,
                            request.Count);

                        PackageSender.SendPacket(peer, new ResUpdatePlayerTeam()
                        {
                            Team = new Team()
                            {
                                Id = player.Id,
                                Members = NetworkPlayerTeamController.PlayerTeams[token]
                            }
                        });
                    }
                    else
                    {

                        //This may trigger 
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
        timer = 0;
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
        var goldMine = NetworkInventoryController.CreateInventory("goldMine");
        goldMine.AddItem(MaterialType.CATAPHRACT_ARMOR.ID(), 1);
        goldMine.AddItem(MaterialType.BONE.ID(), 65);
        goldMine.AddItem(MaterialType.CHAINMAIL_HELMET.ID(), 1);

        Structures.Add(0, new MapStructure()
        {
            Structure = new Structure()
            {
                Id = 0,
                x = 0,
                y = 0,
                InventoryId = goldMine.Id
            },
            TroopCount = 3,
            MaxTroopCount = 5,
        });
    }

    int troopSpawnTimer = 30 * 180;
    int timer = 0;

    public void OnUpdate(float deltaTime)
    {
        timer++;
        if (timer < troopSpawnTimer) return;
        timer = 0;
        foreach (var structure in Structures)
        {
            var mapStructure = structure.Value;
            if (random.NextDouble() >= 0.40)
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