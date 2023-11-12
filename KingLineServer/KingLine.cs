using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class ConnectedPeer
{
    public int Id { get; }
    public string Token { get; }
    public NetPeer Peer { get; }
    public ConnectedPeer(NetPeer peer, string token)
    {
        this.Id = peer.Id;
        this.Peer = peer;
        this.Token = token;
    }
}

public class KingLine : INetEventListener
{

    private static Dictionary<int, ConnectedPeer> _connectedPeers = new Dictionary<int, ConnectedPeer>();

    public static int Multiplier = 100;

    private static ConnectionData connectionData = new ConnectionData();

    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

    private static KingLine _instance;
    public static KingLine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KingLine();
                Console.WriteLine("Initialize Kingline Class");
            }
            return _instance;
        }

    }

    private List<INetworkController> INetworkControllers;
    NetManager server;
    static void Main(string[] args)
    {
        Instance.Run();
    }
    public static string GetPlayerToken(int Id)
    {
        return _connectedPeers[Id].Token;
    }

    public KingLine()
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
                new NetworkPlayerLevelController(),
                new NetworkPlayerTeamController()
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
        INetworkControllers.ForEach(c => c.OnUpdate(1f / Time.TARGET_FPS));
    }


    public void OnConnectionRequest(ConnectionRequest request)
    {
        var data = request.Data;
        var version = data.GetString();
        var userName = data.GetString(16);
        var token = data.GetString(32);
        try
        {
            if (server.ConnectedPeersCount < 1000)
            {
                if (version == connectionData.Version)
                {
                    var peer = request.Accept();

                    _connectedPeers.Add(peer.Id, new ConnectedPeer(peer, token));
                    INetworkControllers.ForEach(t => t.OnPeerConnectionRequest(peer, token, userName));
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
        _connectedPeers.Remove(peer.Id);

        INetworkControllers.ForEach(t => t.OnPeerDisconnected(peer));
    }

    public void OnPeerConnected(NetPeer peer)
    {
        INetworkControllers.ForEach(t => t.OnPeerConnected(peer));
    }
}