using KingLineServer.Network;
using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KingLineServer
{
    class Time
    {
        public const int TARGET_FPS = 30;

        private readonly Stopwatch stopwatch = new();
        private ulong nextTickId = 0;

        // The stopwatch time right now
        public double Now => stopwatch.Elapsed.TotalSeconds;
        // The stopwatch time at the beginning of this tick
        public double TickTime { get; private set; }
        // The ID of the current tick
        public ulong TickId { get; private set; }

        public void Start()
        {
            stopwatch.Start();
        }

        public bool ShouldTick()
        {
            bool shouldTick = Now * TARGET_FPS > nextTickId;

            if (shouldTick)
            {
                TickTime = Now;
                TickId = nextTickId++;
            }

            return shouldTick;
        }
    }



   
    public class KingLineServer : INetEventListener
    {
        public Dictionary<NetPeer, Player> Players = new Dictionary<NetPeer, Player>();
        public Dictionary<int, Structure> Structures = new Dictionary<int, Structure>();

        public Dictionary<string, ItemStack[]> PlayerItems = new Dictionary<string, ItemStack[]>();

        static ConnectionData connectionData = new ConnectionData();
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        NetManager server;
        static void Main(string[] args)
        {
            new KingLineServer().Run();
        }
        public KingLineServer()
        {
            server = new NetManager(this);
            server.DisconnectTimeout = 10000;
            _netPacketProcessor = new NetPacketProcessor();
            _netPacketProcessor.RegisterNestedType(() =>
            {
                return new Player();
            });
            _netPacketProcessor.RegisterNestedType(() =>
            {
                return new Structure();
            });
            InitStructures();
            _netPacketProcessor.SubscribeReusable<ReqPlayers, NetPeer>(OnRequestPlayers);
            _netPacketProcessor.SubscribeReusable<ResPlayerPosition, NetPeer>(OnRequestPositionUpdate);
            _netPacketProcessor.SubscribeReusable<ReqPlayerMove, NetPeer>(OnRequestPlayerMove);
            _netPacketProcessor.SubscribeReusable<ReqStructures, NetPeer>(OnRequestStructures);
            PackageSender.PacketProcessor = _netPacketProcessor;
        }

        private void InitStructures()
        {
            Structures.Add(0, new Structure()
            {
                Id = 0,
                x = 0,
                y = 0,
            });
        }

        private void OnRequestStructures(ReqStructures request, NetPeer peer)
        {
            var packet = new ResStructures();
            packet.Structures = Structures.Values.ToArray();
            PackageSender.SendPacket(peer, packet);
        }

        public void Run()
        {
            Cw.Log("KING LINE SERVER");
            server.Start(connectionData.Port);
            Cw.Log($"\tServer Started | Port : {connectionData.Port}\n\tVersion:{connectionData.Version}\n\tReady for clients!");
            CancellationTokenSource cts = new();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
                e.Cancel = true;
            };

            Time time = new();

            time.Start();

            OnStart();
            while (!cts.IsCancellationRequested)
            {
                while (time.ShouldTick())
                {
                    OnUpdate();
                }
                Thread.Sleep(1);
            }
            OnExit();
        }
        private void OnStart() {
            Cw.Log("\tOnStart...",ConsoleColor.Magenta);
        }
        private void OnExit() { 
            Cw.Log("\tOn Exit...",ConsoleColor.Magenta);
        }
        private void OnUpdate()
        {
            server.PollEvents();
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
            var target = this.Players.FirstOrDefault(t => t.Value.Id == peer.Id);
            this.Players[peer].x = position.x;
            this.Players[peer].y = position.y;
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

        public bool ServerLoop = true;


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

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var data = request.Data;
            var version = data.GetString();
            var userName = data.GetString(16);
            try
            {
                if (server.ConnectedPeersCount < 1000)
                {
                    if (version == connectionData.Version)
                    {
                        var peer = request.Accept();
                        Players.Add(peer, new Player()
                        {
                            Name = userName,
                            Id = peer.Id,
                            x = 0,
                            y = 0,
                            speed = 1.5f,
                            targetX = 0,
                            targetY = 0,
                        });
                        PackageSender.SendPacket(peer, new ResPeerId { Id = peer.Id });
                        Cw.Log($"\tPeer {peer.Id} Client {userName} connected.", ConsoleColor.Green);
                    }
                    else
                    {
                        var versionError = $"Version Error: " +
                            $"Server version is {connectionData.Version} your version is {version}";
                        Cw.Log(versionError, ConsoleColor.Red);
                        request.Reject(Encoding.ASCII.GetBytes(versionError));
                    }
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("$SERVER_IS_FULL");
                    request.Reject(bytes);
                }
            }
            catch (Exception e)
            {
                Cw.Log("OnConnectionRequest: " + e.ToString(), ConsoleColor.Red);
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Cw.Log("OnNetworkError:" + socketError, ConsoleColor.Red);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            try
            {
                _netPacketProcessor.ReadAllPackets(reader, peer);
            }
            catch (Exception e)
            {
                Cw.Log("OnNetworkReceive:" + e.ToString(), ConsoleColor.Red);
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            try
            {
                Cw.Log("OnNetworkReceiveUnconnected" + reader.GetString(), ConsoleColor.Red);
            }
            catch (Exception e)
            {
                Cw.Log("OnNetworkReceiveUnconnected: " + e.ToString(), ConsoleColor.Red);
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
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
    }
}