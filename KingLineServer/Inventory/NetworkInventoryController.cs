using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;

public class NetworkInventoryController : INetworkController
{
    public static Dictionary<string, NetworkInventory> Inventories = new Dictionary<string, NetworkInventory>();

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.RegisterNestedType(() =>
        {
            return new ItemStack();
        });
        processor.SubscribeReusable<ReqInventory, NetPeer>(OnRequestInventory);
        processor.SubscribeReusable<ReqInventoryMove, NetPeer>(OnRequestInventoryMove);
        processor.SubscribeReusable<ReqMineStone, NetPeer>(OnRequestMineStone);
        processor.SubscribeReusable<ReqMineBone, NetPeer>(OnRequestMineBone);
    }

    private void OnRequestMineBone(ReqMineBone request, NetPeer peer)
    {
        InventoryAdd(peer, MaterialType.BONE.ID(), 1);
        NetworkPlayerLevelController.AddXp(peer, 1);
    }
    private void OnRequestMineStone(ReqMineStone request, NetPeer peer)
    {
        InventoryAdd(peer, MaterialType.STONE.ID(), 1);
        NetworkPlayerLevelController.AddXp(peer, 2);
    }

    public void InventoryAdd(NetPeer peer, int id, short count)
    {
        var player = NetworkPlayerController.Players[peer];
        NetworkInventory inventory = Inventories[player.Token];
        if (inventory.AddItem(id, count))
        {
            PackageSender.SendPacket(peer, new ResInventoryAdd()
            {
                Count = count,
                Id = id
            });
        }
    }

    private bool IsGearIndex(int index)
    {
        return index is NetworkInventory.HELMET_SLOT_INDEX or NetworkInventory.ARMOR_SLOT_INDEX
            or NetworkInventory.HAND_SLOT_INDEX;
    }

    private void OnRequestInventoryMove(ReqInventoryMove request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var inventory = Inventories[player.Token];

        if (inventory.MoveItem(request.FromIndex, request.ToIndex))
        {
            var response = new ResInventoryMove()
            {
                ToIndex = request.ToIndex,
                FromIndex = request.FromIndex
            };

            PackageSender.SendPacket(peer, response);


            if (IsGearIndex(request.ToIndex) || IsGearIndex(request.FromIndex))
            {
                foreach (var p in NetworkPlayerController.Players)
                {
                    var netPeer = p.Key;
                    if (netPeer.Id != peer.Id)
                    {
                        var targetPlayer = GetOrCreateInventory(player.Token);
                        PackageSender.SendPacket(netPeer, new ResRemoteInventory()
                        {
                            Id = peer.Id,
                            Items = new ItemStack[3]
                            {
                                targetPlayer.GetHelmet(),
                                targetPlayer.GetArmor(),
                                targetPlayer.GetHand(),
                            }
                        });
                    }
                }
            }
        }
    }

    private void OnRequestInventory(ReqInventory request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var response = new ResInventory();
        var inventory = GetOrCreateInventory(player.Token);
        response.Items = inventory.Items;
        PackageSender.SendPacket(peer, response);
    }

    public NetworkInventory GetOrCreateInventory(string token)
    {
        if (Inventories.TryGetValue(token, out var inv))
        {
            return inv;
        }
        else
        {
            var inventory = new NetworkInventory();
            inventory.SetItem(NetworkInventory.HELMET_SLOT_INDEX, MaterialType.PEASANT_CAP.ID());
            inventory.SetItem(NetworkInventory.ARMOR_SLOT_INDEX, MaterialType.PEASANT_CLOTHING.ID());
            inventory.SetItem(NetworkInventory.HAND_SLOT_INDEX, MaterialType.BONE_CLUP.ID());
            inventory.AddItem(MaterialType.STONE_PICKAXE.ID());
            inventory.AddItem(MaterialType.IRON_PICKAXE.ID());
            inventory.AddItem(MaterialType.STEEL_PICKAXE.ID());
            Inventories.Add(token, inventory);
            return inventory;
        }
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        //TODO FIX LATER  This will probably called before player requires inventory.
        var joinedPlayerInventory = GetOrCreateInventory(NetworkPlayerController.Players[peer].Token);
        var response = new ResRemoteInventory()
        {
            Id = peer.Id,
            Items = new ItemStack[3]
            {
                joinedPlayerInventory.GetHelmet(),
                joinedPlayerInventory.GetArmor(),
                joinedPlayerInventory.GetHand(),
            }
        };

        foreach (var p in NetworkPlayerController.Players)
        {
            var netPeer = p.Key;
            if (netPeer.Id != peer.Id)
            {
                var player = p.Value;
                var targetPlayer = GetOrCreateInventory(player.Token);
                PackageSender.SendPacket(peer, new ResRemoteInventory()
                {
                    Id = netPeer.Id,
                    Items = new ItemStack[3]
                    {
                        targetPlayer.GetHelmet(),
                        targetPlayer.GetArmor(),
                        targetPlayer.GetHand(),
                    }
                });
                PackageSender.SendPacket(p.Key, response);
            }

        }
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
}
