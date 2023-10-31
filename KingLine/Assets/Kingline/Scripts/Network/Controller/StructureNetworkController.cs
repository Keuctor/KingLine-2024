using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class StructureNetworkController : NetworkController
{
    [NonSerialized]
    public UnityEvent OnStructureResponse = new();

    [NonSerialized]
    public Structure[] Structures = Array.Empty<Structure>();

    public override void OnPeerDisconnected(NetPeer peer)
    {
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResStructures>(OnStructuresResponse);
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqStructures());
    }


    public override void OnExit()
    {
    }

    public override void OnStart()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        
    }

    private void OnStructuresResponse(ResStructures obj)
    {
        Structures = obj.Structures;
        OnStructureResponse?.Invoke();
    }
}