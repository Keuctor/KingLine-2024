using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetEventListener
{
    public static int LocalPlayerPeerId;

    [SerializeField]
    private ConnectionDataSO m_connectionData;

    [SerializeField]
    private ConnectionHandlerUI m_connectionHandlerUI;

    private NetManager m_client;
    private bool m_connectionFirstHeartBeatReceived;

    private ConnectionHandlerUI m_connectionHandlerUIInstance;

    private bool m_isServerStarted;

    private EventBasedNetListener m_listener;

    public Action OnConnectedToServer;
    public Action OnDisconnectedFromServer;

    public NetPeer Server;

    private readonly NetDataWriter writer = new();

    public NetPacketProcessor NetPacketProcessor { get; } = new();


    private void Awake()
    {
        CreateConnectionUI();
        InitializeNetPacketProcessor();
    }

    private void FixedUpdate()
    {
        if (m_isServerStarted)
            m_client.PollEvents();
    }

    private void OnDisable()
    {
        if (m_isServerStarted) m_client.Stop();
    }


    public void OnPeerConnected(NetPeer peer)
    {
        Server = peer;
        DestroyConnectionUI();
        LoadingHandler.Instance.ShowLoading("Connected to server");
        LoadingHandler.Instance.HideAfterSeconds(0.5f);
        OnConnectedToServer?.Invoke();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        GlobalCanvas.Instance.SetLatency(-1);
        Debug.Log(m_connectionFirstHeartBeatReceived);

        if (m_connectionFirstHeartBeatReceived)
            LoadingHandler.Instance.ShowLoading("Disconnected from server " + disconnectInfo.Reason);
        else
            LoadingHandler.Instance.ShowLoading($"Can't connect to server try again later: {disconnectInfo.Reason}");

        LoadingHandler.Instance.HideAfterSeconds(4f);
        m_isServerStarted = false;
        m_connectionFirstHeartBeatReceived = false;
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
        m_connectionFirstHeartBeatReceived = true;
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
        m_client.Connect(m_connectionData.Adress, m_connectionData.Port,
            writer);
        m_isServerStarted = true;
    }

    private void InitializeNetPacketProcessor()
    {
        NetPacketProcessor.RegisterNestedType(() => new Player());
        NetPacketProcessor.RegisterNestedType(() => new Structure());
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