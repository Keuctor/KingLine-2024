
using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;

public class NetworkPlayerController : INetworkController
{
    public static Dictionary<NetPeer, Player> Players = new Dictionary<NetPeer, Player>();


    public void Subscribe(NetPacketProcessor processor)
    {
        processor.RegisterNestedType(() =>
        {
            return new Player();
        });
        processor.SubscribeReusable<ReqPlayers, NetPeer>(OnRequestPlayers);
        processor.SubscribeReusable<ResPlayerPosition, NetPeer>(OnRequestPositionUpdate);
        processor.SubscribeReusable<ReqPlayerMove, NetPeer>(OnRequestPlayerMove);
        processor.SubscribeReusable<ReqSellItem, NetPeer>(OnSellItem);
    }

    public static Player GetPlayer(NetPeer peer)
    {
        return Players[peer];
    }

    private void OnSellItem(ReqSellItem request, NetPeer peer)
    {

        var player = Players[peer];
        var inventory = NetworkInventoryController.GetInventory(peer);
        var targetItemStack = inventory.Items[request.Index];
        if (targetItemStack.Id != -1 && targetItemStack.Count >= request.Count)
        {
            var itemInfo = ItemRegistry.GetItem(targetItemStack.Id);
            player.Gold += (itemInfo.Value * request.Count);
            inventory.RemoveItem(request.Index, request.Count);

            PackageSender.SendPacket(peer, new ResPlayerCurrency()
            {
                NewCurrency = player.Gold,
            }, DeliveryMethod.ReliableUnordered);

            PackageSender.SendPacket(peer, new ResInventoryRemove()
            {
                InventoryId = inventory.Id,
                Count = request.Count,
                Index = request.Index
            }, DeliveryMethod.ReliableUnordered);
        }
    }

    private void OnRequestPlayerMove(ReqPlayerMove request, NetPeer peer)
    {
        var target = Players[peer];
        target.TargetY = request.x;
        target.TargetX = request.y;
        var packet = new ResPlayerMove()
        {
            Id = peer.Id,
            x = request.x,
            y = request.y,
        };

        foreach (var p in Players)
        {
            if (p.Key.Id == peer.Id)
                continue;
            PackageSender.SendPacket(p.Key, packet);
        }
    }
    public void BroadcastPackage<T>(T packet) where T : class, new()
    {
        foreach (var p in Players)
        {
            PackageSender.SendPacket(p.Key, packet);
        }
    }

    private void OnRequestPositionUpdate(ResPlayerPosition position, NetPeer peer)
    {
        var target = Players.FirstOrDefault(t => t.Value.Id == peer.Id);
        Players[peer].X = position.x;
        Players[peer].Y = position.y;
        var packet = new ResPlayerPosition()
        {
            Id = peer.Id,
            x = position.x,
            y = position.y,
        };
        BroadcastPackage(packet);
    }

    private void OnRequestPlayers(ReqPlayers request, NetPeer peer)
    {
        var packet = new ResPlayers()
        {
            Players = Players.Values.ToArray(),
        };
        PackageSender.SendPacket(peer, packet);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        var player = Players[peer];
        foreach (var p in Players)
        {
            if (p.Key.Id != peer.Id)
            {
                var package = new ResPlayerJoin() { Player = player };
                PackageSender.SendPacket(p.Key, package);
            }
        }
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
        var player = Players[peer];
        foreach (var p in Players)
        {
            if (p.Key.Id != peer.Id)
            {
                PackageSender.SendPacket(p.Key, new ResPlayerLeave() { Player = player });
            }
        }
        if (Players.Remove(peer, out _))
        {
            Cw.Log($"\tPeer Id: {peer.Id} Name: {player.Name} Disconnected", ConsoleColor.Gray);
        }
        else
        {
            Cw.Log($"\tClient does not exist in table", ConsoleColor.Red);
        }
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
        var player = new Player()
        {
            Name = username,
            Id = peer.Id,
            X = 0,
            Y = 0,
            Speed = 1.5f,
            TargetY = 0,
            TargetX = 0,
            Gold = 0,
        };

        NetworkInventory inventory = NetworkInventoryController.CreateInventory(username);

        inventory.SetGear(new ItemStack[]
        {
            new ItemStack(MaterialType.CATAPHRACT_HELMET.ID()),
            new ItemStack(MaterialType.LEATHER_JACKET_ARMOR.ID()),
            new ItemStack(MaterialType.KNIGHT_SWORD_WEAPON.ID())
        });  
            
        inventory.AddItem(MaterialType.TOOL_STONE_PICKAXE.ID());
        inventory.AddItem(MaterialType.TOOL_IRON_PICKAXE.ID());
        inventory.AddItem(MaterialType.TOOL_STEEL_PICKAXE.ID());
        inventory.AddItem(MaterialType.STONE.ID(),100);
        player.InventoryId = inventory.Id;
        Players.Add(peer, player);
    }

    public void OnExit()
    {
    }

    public void OnStart()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var keyValue in Players)
        {
            var player = keyValue.Value;
            var peer = keyValue.Key;

            if (Math.Abs(player.X - player.TargetY) <= float.Epsilon ||
                Math.Abs(player.Y - player.TargetX) <= float.Epsilon)
            {
                continue;
            }
            var newPos = MoveTowards(new Vector2(player.X, player.Y),
                new Vector2(player.TargetY, player.TargetX), deltaTime * player.Speed);
            player.X = newPos.X;
            player.Y = newPos.Y;
        }
    }

    public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
    {
        float toVector_x = target.X - current.X;
        float toVector_y = target.Y - current.Y;

        float sqDist = toVector_x * toVector_x + toVector_y * toVector_y;

        if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
            return target;

        float dist = (float)Math.Sqrt(sqDist);

        return new Vector2(current.X + toVector_x / dist * maxDistanceDelta,
            current.Y + toVector_y / dist * maxDistanceDelta);
    }

}