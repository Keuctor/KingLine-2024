
using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

public partial class Player
{
    public string UniqueIdendifier = string.Empty;
}
public partial class Player : INetSerializable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float targetX { get; set; }
    public float targetY { get; set; }
    public float speed { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(x);
        writer.Put(y);
        writer.Put(targetX);
        writer.Put(targetY);
        writer.Put(speed);
    }
    public void Deserialize(NetDataReader reader)
    {
        this.Id = reader.GetInt();
        this.Name = reader.GetString(16);
        this.x = reader.GetFloat();
        this.y = reader.GetFloat();
        this.targetX = reader.GetFloat();
        this.targetY = reader.GetFloat();
        this.speed = reader.GetFloat();
    }
}

public class ReqPlayers
{

}
public class ResPlayers
{
    public Player[] Players { get; set; }
}
public class ResPlayerPosition
{
    public int Id { get; set; }
    public float x { get; set; }
    public float y { get; set; }
}


public class ResPlayerJoin
{
    public Player Player { get; set; }
}

public class ResPlayerLeave
{
    public Player Player { get; set; }
}
public class ReqPlayerMove
{
    public float x { get; set; }
    public float y { get; set; }
}

public class ResPlayerMove
{
    public int Id { get; set; }
    public float y { get; set; }
    public float x { get; set; }
}

namespace KingLineServer.Inventory
{
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
            BroadcastPackage(packet);
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
                    PackageSender.SendPacket(peer, new ResPlayerLeave() { Player = player });
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

        public void OnPeerConnectionRequest(NetPeer peer, string idendifier,string username)
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
                UniqueIdendifier = idendifier
            });
        }

        public void OnExit()
        {
        }

        public void OnStart()
        {
        }
    }
}
