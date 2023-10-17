using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;



public class NetworkInventoryController : INetworkController
{
    public static Dictionary<string, ItemStack[]> PlayerItems = new Dictionary<string, ItemStack[]>();
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
        ItemStack[] items = PlayerItems[player.Token];

        bool found = false;
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.Id == id)
            {
                item.Count += count;
                found = true;
                break;
            }
        }
        if (!found)
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i].Id == -1)
                {
                    items[i].Id = id;
                    items[i].Count = count;
                    break;
                }
            }
        }
        PlayerItems[player.Token] = items;
        PackageSender.SendPacket(peer, new ResInventoryAdd()
        {
            Count = count,
            Id = id
        });
    }


    private void OnRequestInventoryMove(ReqInventoryMove request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        if (PlayerItems.TryGetValue(player.Token, out ItemStack[] items))
        {
            var item = items[request.FromIndex];
            if (item != null)
            {
                if (item.Id != -1)
                {
                    var to = items[request.ToIndex];
                    if (to != null)
                    {
                        if (to.Id == -1)
                        {
                            items[request.ToIndex] = item;
                            items[request.FromIndex] = new ItemStack()
                            {
                                Count = 0,
                                Id = -1
                            };
                        }
                    }
                }
            }

            PlayerItems[player.Token] = items;
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
        if (PlayerItems.TryGetValue(player.Token, out ItemStack[] items))
        {
            response.Items = items;
        }
        else
        {
            //Last 4 will be helmet / body / hand 
            response.Items = new ItemStack[28];
            for (int i = 0; i < 28; i++)
            {
                response.Items[i] = new ItemStack()
                {
                    Count = 0,
                    Id = -1,
                };
            }

            response.Items[25] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.PEASANT_CAP.ID(),
            };
            response.Items[26] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.PEASANT_CLOTHING.ID(),
            };
            response.Items[27] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.BONE_CLUP.ID(),
            };
            response.Items[0] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.STONE_PICKAXE.ID(),
            };
            response.Items[1] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.IRON_PICKAXE.ID(),
            };
            response.Items[2] = new ItemStack()
            {
                Count = 1,
                Id = MaterialType.STEEL_PICKAXE.ID(),
            };
            PlayerItems.Add(player.Token, response.Items);
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
