using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryNetworkController : NetworkController
{
    public Dictionary<ulong, NetworkInventory> Inventories = new Dictionary<ulong, NetworkInventory>();
    public static NetworkInventory LocalInventory;
    
    public readonly UnityEvent OnInventory = new();

    public override void OnPeerDisconnected(NetPeer peer)
    {
   
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqInventory());
    }

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResInventory>(OnInventoryResponse);
        processor.SubscribeReusable<ResInventoryAdd>(OnInventoryAdd);
        processor.SubscribeReusable<ResInventoryRemove>(OnInventoryRemove);
        processor.SubscribeReusable<ResRemoteInventory>(OnRemoteInventory);
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

    private void OnInventoryResponse(ResInventory result)
    {
        Inventories.Add(result.Id,new NetworkInventory(result.Id.ToString(), result.Items,result.Gear));
        
        LocalInventory = Inventories[result.Id];
    }
    private void OnInventoryAdd(ResInventoryAdd response)
    {
        this.Inventories[response.InventoryId].AddItem(response.Id, response.Count);
    }

    private void OnInventoryRemove(ResInventoryRemove response)
    {
        this.Inventories[response.InventoryId].RemoveItem(response.Index, response.Count);
    }


    private void OnRemoteInventory(ResRemoteInventory remoteInventory)
    {
        var inv = new NetworkInventory(remoteInventory.Id.ToString(),null,remoteInventory.Gear);
        Inventories.Add(remoteInventory.Id,inv);
    }


    public static void Sell(int index, short count)
    {
        NetworkManager.Instance.Send(new ReqSellItem()
        {
            Index = index,
            Count = count,
        });
    }
}