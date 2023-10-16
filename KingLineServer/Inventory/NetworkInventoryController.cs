using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

public class ItemStack : INetSerializable
{
    public int Id { get; set; }
    public short Count { get; set; }
    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        Count = reader.GetShort();
    }
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Count);
    }
}

public class ReqInventory { }
public class ResInventory
{
    public ItemStack[] Items { get; set; }
}
public class ReqInventoryMove
{
    public short FromIndex { get; set; }
    public short ToIndex { get; set; }
}

public class ResInventoryMove
{
    public short FromIndex { get; set; }
    public short ToIndex { get; set; }
}

public class ResInventoryAdd
{
    public int Id { get; set; }
    public short Count { get; set; }
}
public class ReqMineStone
{
}

public class ReqMineBone
{
}

namespace KingLineServer.Inventory
{
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
            NetworkPlayerLevelController.AddXp(peer, 2);
        }
        private void OnRequestMineStone(ReqMineStone request, NetPeer peer)
        {
            InventoryAdd(peer, 0, 1);
            NetworkPlayerLevelController.AddXp(peer, 500);
        }

        public void InventoryAdd(NetPeer peer, int id, short count)
        {
            var player = NetworkPlayerController.Players[peer];
            ItemStack[] items = PlayerItems[player.UniqueIdendifier];

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
            PlayerItems[player.UniqueIdendifier] = items;
            PackageSender.SendPacket(peer, new ResInventoryAdd()
            {
                Count = count,
                Id = id
            });
        }


        private void OnRequestInventoryMove(ReqInventoryMove request, NetPeer peer)
        {
            var player = NetworkPlayerController.Players[peer];
            if (PlayerItems.TryGetValue(player.UniqueIdendifier, out ItemStack[] items))
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

                PlayerItems[player.UniqueIdendifier] = items;
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
            if (PlayerItems.TryGetValue(player.UniqueIdendifier, out ItemStack[] items))
            {
                response.Items = items;
            }
            else
            {
                response.Items = new ItemStack[25];
                for (int i = 0; i < 25; i++)
                {
                    response.Items[i] = new ItemStack()
                    {
                        Count = 0,
                        Id = -1,
                    };
                }
                PlayerItems.Add(player.UniqueIdendifier, response.Items);
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
}
