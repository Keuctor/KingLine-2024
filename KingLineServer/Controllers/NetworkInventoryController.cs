using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkInventoryController : INetworkController
{
    public static Dictionary<ulong, NetworkInventory> Inventories = new Dictionary<ulong, NetworkInventory>();
    public void Subscribe(NetPacketProcessor processor)
    {
        processor.RegisterNestedType(() =>
        {
            return new ItemStack();
        });
        processor.SubscribeReusable<ReqInventory, NetPeer>(OnRequestInventory);
        processor.SubscribeReusable<ReqMineStone, NetPeer>(OnRequestMineStone);
        processor.SubscribeReusable<ReqMineBone, NetPeer>(OnRequestMineBone);
    }

    private void OnRequestMineBone(ReqMineBone request, NetPeer peer)
    {
        InventoryAdd(peer, MaterialType.BONE.ID(), 1);
    }
    private void OnRequestMineStone(ReqMineStone request, NetPeer peer)
    {
        InventoryAdd(peer, MaterialType.STONE.ID(), 1);
    }

    public static bool InventoryAdd(NetPeer peer, int id, short count)
    {
        var invId = NetworkPlayerController.GetPlayer(peer).InventoryId;
        NetworkInventory inventory = Inventories[invId];
        if (inventory.AddItem(id, count))
        {
            PackageSender.SendPacket(peer, new ResInventoryAdd()
            {
                Count = count,
                Id = id,
                InventoryId = inventory.Id,
            });
            return true;
        }
        return false;
    }


    private void OnRequestInventory(ReqInventory request, NetPeer peer)
    {
        var token = KingLine.GetPlayerToken(peer.Id);

        var response = new ResInventory();
        var inventory = GetInventory(peer);
        response.Id = inventory.Id;
        response.Items = inventory.Items;
        response.Gear = inventory.Gear;
        PackageSender.SendPacket(peer, response);

        //Send also other players inventories to this player
        foreach (var p in NetworkPlayerController.Players)
        {
            var netPeer = p.Key;
            if (netPeer.Id != peer.Id)
            {
                var tkn = KingLine.GetPlayerToken(p.Key.Id);
                var targetInv = GetInventory(p.Key);
                PackageSender.SendPacket(peer, new ResRemoteInventory()
                {
                    Id = targetInv.Id,
                    Gear = new ItemStack[3]
                    {
                        targetInv.GetHelmet(),
                        targetInv.GetArmor(),
                        targetInv.GetHand(),
                    }
                });
            }
        }
    }

    public static NetworkInventory CreateInventory(string name)
    {
        var inventory = new NetworkInventory(name);
        Inventories.Add(inventory.Id, inventory);
        return inventory;
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        foreach (var p in NetworkPlayerController.Players)
        {
            if (peer.Id != p.Key.Id)
            {
                var myInv = GetInventory(peer);
                PackageSender.SendPacket(p.Key, new ResRemoteInventory()
                {
                    Id = myInv.Id,
                    Gear = new ItemStack[3]
                    {
                        myInv.GetHelmet(),
                        myInv.GetArmor(),
                        myInv.GetHand(),
                    }
                });
            }
        }
    }


    public static NetworkInventory GetInventory(NetPeer peer)
    {
        var player = NetworkPlayerController.GetPlayer(peer);
        return Inventories[player.InventoryId];
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public void OnExit()
    {
    }

    public void OnStart()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }
}
