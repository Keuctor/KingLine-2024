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
    }
    private void OnRequestMineStone(ReqMineStone request, NetPeer peer)
    {
        InventoryAdd(peer, MaterialType.STONE.ID(), 1);
    }

    public static bool InventoryAdd(NetPeer peer, int id, short count)
    {
        var token = KingLine.GetPlayerToken(peer.Id);
        NetworkInventory inventory = Inventories[token];
        if (inventory.AddItem(id, count))
        {
            PackageSender.SendPacket(peer, new ResInventoryAdd()
            {
                Count = count,
                Id = id
            });
            return true;
        }
        return false;
    }

    private bool IsGearIndex(int index)
    {
        return index is NetworkInventory.HELMET_SLOT_INDEX or NetworkInventory.ARMOR_SLOT_INDEX
            or NetworkInventory.HAND_SLOT_INDEX;
    }

    private void OnRequestInventoryMove(ReqInventoryMove request, NetPeer peer)
    {
        var token = KingLine.GetPlayerToken(peer.Id);

        var inventory = Inventories[token];

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
        var token = KingLine.GetPlayerToken(peer.Id);

        var response = new ResInventory();
        var inventory = GetOrCreateInventory(token);
        response.Items = inventory.Items;
        PackageSender.SendPacket(peer, response);

        //Send also other players inventories to this player
        foreach (var p in NetworkPlayerController.Players)
        {
            var netPeer = p.Key;
            if (netPeer.Id != peer.Id)
            {
                var tkn = KingLine.GetPlayerToken(p.Key.Id);
                var targetPlayer = GetOrCreateInventory(tkn);
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
            inventory.SetItem(NetworkInventory.HELMET_SLOT_INDEX, MaterialType.CATAPHRACT_HELMET.ID());
            inventory.SetItem(NetworkInventory.ARMOR_SLOT_INDEX, MaterialType.LEATHER_JACKET_ARMOR.ID());
            inventory.SetItem(NetworkInventory.HAND_SLOT_INDEX, MaterialType.KNIGHT_SWORD_WEAPON.ID());
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
        foreach (var p in NetworkPlayerController.Players)
        {
            if (peer.Id != p.Key.Id)
            {
                var myInv = GetOrCreateInventory(KingLine.GetPlayerToken(peer.Id));
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
