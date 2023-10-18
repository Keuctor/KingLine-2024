using System;
using DG.Tweening;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryNetworkController : INetworkController
{
    public ItemStack[] Items = new ItemStack[28];

    public readonly UnityEvent OnGearChange = new();
    public readonly UnityEvent OnInventory = new();
    public readonly UnityEvent<int,int> OnAddItem = new();

    private void OnInventoryMove(ResInventoryMove obj)
    {
        Items[obj.ToIndex] = Items[obj.FromIndex];
        Items[obj.FromIndex] = new ItemStack()
        {
            Count = 0,
            Id = -1,
        };
        if (obj.ToIndex >= 25 || obj.FromIndex >= 25)
        {
            OnGearChange?.Invoke();
        }
    }
    

    private void OnInventoryResponse(ResInventory result)
    {
        Items = result.Items;
        OnInventory?.Invoke();
    }
  

    private void OnInventoryAdd(ResInventoryAdd response)
    {
        bool found = false;
        for (var i = 0; i < Items.Length - 3; i++)
        {
            var item = Items[i];
            if (item.Id == response.Id)
            {
                item.Count += response.Count;
                found = true;
                break;
            }
        }
        if (!found)
        {
            for (var i = 0; i < Items.Length - 3; i++)
            {
                if (Items[i].Id == -1)
                {
                    Items[i].Id = response.Id;
                    Items[i].Count = response.Count;
                    break;
                }
            }
        }
        OnAddItem?.Invoke(response.Id,response.Count);
    }
 
    public void OnPeerDisconnected(NetPeer peer)
    {
       
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqInventory());
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResInventory>(OnInventoryResponse);
        processor.SubscribeReusable<ResInventoryMove>(OnInventoryMove);
        processor.SubscribeReusable<ResInventoryAdd>(OnInventoryAdd);
    }

    public void OnExit()
    {
    }

    public void OnStart()
    {
    }
}