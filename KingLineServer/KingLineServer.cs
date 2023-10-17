using KingLineServer.Network;
using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;



namespace KingLineServer
{
    public class KingLineServer : INetEventListener
    {
        static ConnectionData connectionData = new ConnectionData();
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        private List<INetworkController> INetworkControllers;
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
            INetworkControllers = new List<INetworkController>
            {
                new NetworkPlayerController(),
                new NetworkStructureController(),
                new NetworkInventoryController(),
                new NetworkPlayerProgressionController(),
                new NetworkPlayerLevelController()
            };
            INetworkControllers.ForEach(c => c.Subscribe(_netPacketProcessor));
            PackageSender.PacketProcessor = _netPacketProcessor;
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

        private void OnStart()
        {
            Cw.Log("\tOnStart...", ConsoleColor.Magenta);
            INetworkControllers.ForEach(c => c.OnStart());
        }
        private void OnExit()
        {
            Cw.Log("\tOn Exit...", ConsoleColor.Magenta);
            INetworkControllers.ForEach(c => c.OnExit());
        }
        private void OnUpdate()
        {
            server.PollEvents();
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var data = request.Data;
            var version = data.GetString();
            var userName = data.GetString(16);
            var idendifier = data.GetString(32);
            try
            {
                if (server.ConnectedPeersCount < 1000)
                {
                    if (version == connectionData.Version)
                    {
                        var peer = request.Accept();
                        INetworkControllers.ForEach(t => t.OnPeerConnectionRequest(peer, idendifier,userName));
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
            INetworkControllers.ForEach(t => t.OnPeerDisconnected(peer));
        }

        public void OnPeerConnected(NetPeer peer)
        {
            INetworkControllers.ForEach(t => t.OnPeerConnected(peer));
        }
    }
}