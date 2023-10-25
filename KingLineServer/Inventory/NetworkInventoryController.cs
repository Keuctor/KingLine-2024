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
            
            //Send my gear changes to other players
            if (IsGearIndex(request.ToIndex) || IsGearIndex(request.FromIndex))
            {
                foreach (var p in NetworkPlayerController.Players)
                {
                    var netPeer = p.Key;
                    if (netPeer.Id != peer.Id)
                    {
                        PackageSender.SendPacket(netPeer, new ResRemoteInventory()
                        {
                            Id = peer.Id,
                            Items = new ItemStack[3]
                            {
                                inventory.GetHelmet(),
                                inventory.GetArmor(),
                                inventory.GetHand(),
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

        //Send also other players inventories to this player
        foreach (var p in NetworkPlayerController.Players)
        {
            var netPeer = p.Key;
            if (netPeer.Id != peer.Id)
            {
                var targetPlayer = GetOrCreateInventory(p.Value.Token);
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
            }
        }
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
            inventory.SetItem(NetworkInventory.HELMET_SLOT_INDEX, MaterialType.PEASANT_HELMET.ID());
            inventory.SetItem(NetworkInventory.ARMOR_SLOT_INDEX, MaterialType.PEASANT_CLOTHING_ARMOR.ID());
            inventory.SetItem(NetworkInventory.HAND_SLOT_INDEX, MaterialType.BONE_CLUP_WEAPON.ID());
            inventory.AddItem(MaterialType.TOOL_STONE_PICKAXE.ID());
            inventory.AddItem(MaterialType.TOOL_IRON_PICKAXE.ID());
            inventory.AddItem(MaterialType.TOOL_STEEL_PICKAXE.ID());
            inventory.AddItem(MaterialType.TOOL_STEEL_PICKAXE.ID());
            Inventories.Add(token, inventory);
            return inventory;
        }
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        //im gonna request all the other players inv but
        //they don't know about my inventory so i have to send them
        var me = NetworkPlayerController.Players[peer];
        foreach (var p in NetworkPlayerController.Players)
        {
            if (me.Id != p.Key.Id)
            {
                var myInv = GetOrCreateInventory(me.Token);
                PackageSender.SendPacket(p.Key, new ResRemoteInventory()
                {
                    Id = peer.Id,
                    Items = new ItemStack[3]
                    {
                        myInv.GetHelmet(),
                        myInv.GetArmor(),
                        myInv.GetHand(),
                    }
                });
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

    public void OnUpdate(float deltaTime)
    {
    }
}
