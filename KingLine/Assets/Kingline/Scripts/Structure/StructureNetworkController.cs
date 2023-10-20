using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.Events;

public class StructureNetworkController : INetworkController
{
    public UnityEvent OnStructureResponse = new();
    public Structure[] Structures = Array.Empty<Structure>();

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResStructures>(OnStructuresResponse);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqStructures());
    }


    public void OnExit()
    {
    }

    public void OnStart()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        
    }

    private void OnStructuresResponse(ResStructures obj)
    {
        Structures = obj.Structures;
        OnStructureResponse?.Invoke();
    }
}