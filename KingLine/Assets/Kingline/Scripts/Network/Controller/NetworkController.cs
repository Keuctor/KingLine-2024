using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

[CreateAssetMenu]
public abstract class NetworkController : ScriptableObject,INetworkController
{
    public abstract void OnPeerDisconnected(NetPeer peer);
    public abstract void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username);
    public abstract void OnPeerConnected(NetPeer peer);
    public abstract void Subscribe(NetPacketProcessor processor);
    public abstract void OnExit();
    public abstract void OnStart();
    public abstract void OnUpdate(float deltaTime);
}
