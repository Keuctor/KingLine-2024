using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using QFSW.QC;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetEventListener
{
    public static int LocalPlayerPeerId;

    public static NetworkManager Instance;

    [SerializeField]
    private ConnectionDataSO m_connectionData;

    [SerializeField]
    private ConnectionHandlerUI m_connectionHandlerUI;

    private readonly NetDataWriter writer = new();

    [NonSerialized]
    public bool Connected;

    private NetManager m_client;

    private ConnectionHandlerUI m_connectionHandlerUIInstance;

    [Command("game.version")]
    public string GetVersion()
    {
        return m_connectionData.Version;
    }


    private bool m_isServerStarted;

    public List<NetworkController> NetworkControllers = new List<NetworkController>();

    public Action OnConnectedToServer;
    public Action OnDisconnectedFromServer;

    public NetPeer Server;

    public string UniqueKey
    {
        get
        {
            if (m_connectionData.Debug) return Guid.NewGuid().ToString().Substring(0, 12);

            if (!PlayerPrefs.HasKey(nameof(UniqueKey)))
                PlayerPrefs.SetString(nameof(UniqueKey), Guid.NewGuid().ToString().Substring(0, 12));

            return PlayerPrefs.GetString(nameof(UniqueKey));
        }
    }

    public NetPacketProcessor NetPacketProcessor { get; } = new();


    private void FixedUpdate()
    {
        if (m_isServerStarted)
            m_client.PollEvents();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNetPacketProcessor();
            CreateConnectionUI();
        }
    }

    private void Update()
    {
        foreach (var controller in NetworkControllers)
        {
            controller.OnUpdate(Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        if (Instance == this)
            if (m_isServerStarted)
                m_client.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Server = peer;
        DestroyConnectionUI();
        LoadingHandler.Instance.ShowLoading("Connected to server");
        LoadingHandler.Instance.HideAfterSeconds(0.5f);
        OnConnectedToServer?.Invoke();
        NetworkControllers.ForEach(t => t.OnPeerConnected(peer));
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        GlobalCanvas.Instance.SetLatency(-1);
        NetworkControllers.ForEach(t => t.OnPeerDisconnected(peer));
        if (Connected)
            LoadingHandler.Instance.ShowLoading("Disconnected from server " + disconnectInfo.Reason);
        else
            LoadingHandler.Instance.ShowLoading($"Can't connect to server try again later: {disconnectInfo.Reason}");

        LoadingHandler.Instance.HideAfterSeconds(4f);
        m_isServerStarted = false;
        Connected = false;
        OnDisconnectedFromServer?.Invoke();
        Server = null;
        CreateConnectionUI();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        LoadingHandler.Instance.ShowLoading("Network Error");
        LoadingHandler.Instance.ShowLoading("Error: " + socketError);
        LoadingHandler.Instance.HideAfterSeconds(2);
    }


    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod)
    {
        NetPacketProcessor.ReadAllPackets(reader);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint,
        NetPacketReader reader, UnconnectedMessageType messageType)
    {
        LoadingHandler.Instance.ShowLoading($"OnNetworkReceiveUnconnected {reader.GetString()}:{messageType}");
        LoadingHandler.Instance.HideAfterSeconds(2);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        Connected = true;
        GlobalCanvas.Instance.SetLatency(latency);
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        LoadingHandler.Instance.ShowLoading("Connection Request");
        LoadingHandler.Instance.HideAfterSeconds(1f);
    }

    public void DestroyConnectionUI()
    {
        if (m_connectionHandlerUIInstance != null)
        {
            Destroy(m_connectionHandlerUIInstance.gameObject);
            m_connectionHandlerUIInstance = null;
        }
    }

    private void CreateConnectionUI()
    {
        DestroyConnectionUI();
        m_connectionHandlerUIInstance = Instantiate(m_connectionHandlerUI);
        m_connectionHandlerUIInstance.OnConnectClicked += OnConnect;
    }

    private void OnConnect(string userName)
    {
        if (m_isServerStarted)
        {
            LoadingHandler.Instance.ShowLoading("Already connected to server...");
            LoadingHandler.Instance.HideAfterSeconds(1f);
            return;
        }

        LoadingHandler.Instance.ShowLoading("Connecting to server.");
        m_client = new NetManager(this);
        m_client.Start();
        m_client.DisconnectTimeout = 10000;

        writer.Reset();
        writer.Put(m_connectionData.Version);
        writer.Put(userName);
        writer.Put(UniqueKey);
        m_client.Connect(m_connectionData.Adress, m_connectionData.Port,
            writer);
        m_isServerStarted = true;
    }


    private void InitializeNetPacketProcessor()
    {
        NetPacketProcessor.RegisterNestedType(() => new Player());
        NetPacketProcessor.RegisterNestedType(() => new Structure());
        NetPacketProcessor.RegisterNestedType(() => new ItemStack());
        NetPacketProcessor.RegisterNestedType(() => new Skill());
        NetPacketProcessor.RegisterNestedType(() => new TeamMember());
        NetPacketProcessor.RegisterNestedType(() => new Team());

        NetworkControllers.ForEach(t => t.Subscribe(NetPacketProcessor));
        NetPacketProcessor.SubscribeReusable<ResPeerId>(OnPeerIdReceived);
    }

    private void OnPeerIdReceived(ResPeerId resPeerId)
    {
        LocalPlayerPeerId = resPeerId.Id;
        GlobalCanvas.Instance.SetId(resPeerId.Id);
    }

    public void Send<T>(T packet) where T : class, new()
    {
        Server.Send(WritePacket(packet), DeliveryMethod.ReliableOrdered);
    }

    public void SendUnreliable<T>(T packet) where T : class, new()
    {
        Server.Send(WritePacket(packet), DeliveryMethod.Unreliable);
    }

    private NetDataWriter WritePacket<T>(T packet) where T : class, new()
    {
        writer.Reset();
        NetPacketProcessor.Write(writer, packet);
        return writer;
    }
}