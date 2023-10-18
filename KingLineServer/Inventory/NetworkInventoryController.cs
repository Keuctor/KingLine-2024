using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkInventoryController : INetworkController
{
    public static Dictionary<string, NetworkInventory> PlayerItems = new Dictionary<string, NetworkInventory>();

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
        InventoryAdd(peer, 1, 1);
        NetworkPlayerLevelController.AddXp(peer, 1);
    }
    private void OnRequestMineStone(ReqMineStone request, NetPeer peer)
    {
        InventoryAdd(peer, 0, 1);
        NetworkPlayerLevelController.AddXp(peer, 2);
    }

    public void InventoryAdd(NetPeer peer, int id, short count)
    {
        var player = NetworkPlayerController.Players[peer];
        NetworkInventory inventory = PlayerItems[player.Token];
        if (inventory.AddItem(id, count))
        {
            PackageSender.SendPacket(peer, new ResInventoryAdd()
            {
                Count = count,
                Id = id
            });
        }
    }


    private void OnRequestInventoryMove(ReqInventoryMove request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var items = PlayerItems[player.Token];
        if (items.MoveItem(request.FromIndex, request.ToIndex))
        {
            var response = new ResInventoryMove()
            {
                ToIndex = request.ToIndex,
                FromIndex = request.FromIndex
            };
            PackageSender.SendPacket(peer, response);
        }
    }

    private void OnRequestInventory(ReqInventory request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];

        var response = new ResInventory();

        if (PlayerItems.TryGetValue(player.Token, out var inv))
        {
            response.Items = inv.Items;
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
            response.Items = inventory.Items;
            PlayerItems.Add(player.Token, inventory);
        }

        PackageSender.SendPacket(peer, response);
    }

    public void OnPeerDisconnected(NetPeer peer)
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
    }
}
