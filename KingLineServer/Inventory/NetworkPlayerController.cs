
using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;
using System.Runtime.CompilerServices;

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
    }

    private void OnRequestPlayerMove(ReqPlayerMove request, NetPeer peer)
    {
        var target = Players[peer];
        target.targetX = request.x;
        target.targetY = request.y;
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
        Players[peer].x = position.x;
        Players[peer].y = position.y;
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
        Players.Add(peer, new Player()
        {
            Name = username,
            Id = peer.Id,
            x = 0,
            y = 0,
            speed = 1.5f,
            targetX = 0,
            targetY = 0,
            Token = idendifier,
            Currency = 35,
        });
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

            if(Math.Abs(player.x-player.targetX)<=float.Epsilon || 
                Math.Abs(player.y - player.targetY) <= float.Epsilon)
            {
                continue;
            }
            var newPos = MoveTowards(new Vector2(player.x, player.y),
                new Vector2(player.targetX, player.targetY), deltaTime * player.speed);
            player.x = newPos.X;
            player.y = newPos.Y;
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