using System.Collections.Generic;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class InventoryNetworkController : NetworkController
{
    public static NetworkInventory LocalInventory;

    public static Dictionary<int, ItemStack[]> RemoteInventories = new();

    public static bool IsLoaded;
    //id , added, total 
    public readonly UnityEvent<int, int, int> OnAddItem = new();
    
    //index,total
    public readonly UnityEvent<int, int> OnRemoveItem = new();

    public readonly UnityEvent<int> OnGearChange = new();
    public readonly UnityEvent OnInventory = new();

    public override void OnPeerDisconnected(NetPeer peer)
    {
        RemoteInventories.Remove(peer.Id);
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
        processor.SubscribeReusable<ResInventoryMove>(OnInventoryMove);
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


    public static async Task<NetworkInventory> GetInventoryAsync()
    {
        while (LocalInventory == null) await Task.Yield();

        return LocalInventory;
    }

    private bool IsGearIndex(int index)
    {
        return index is NetworkInventory.HELMET_SLOT_INDEX or NetworkInventory.ARMOR_SLOT_INDEX
            or NetworkInventory.HAND_SLOT_INDEX;
    }

    private void OnInventoryMove(ResInventoryMove inventoryMove)
    {
        LocalInventory.MoveItem(inventoryMove.FromIndex, inventoryMove.ToIndex);
        if (IsGearIndex(inventoryMove.FromIndex) || IsGearIndex(inventoryMove.ToIndex))
            OnGearChange?.Invoke(NetworkManager.LocalPlayerPeerId);
    }


    private void OnInventoryResponse(ResInventory result)
    {
        LocalInventory = new NetworkInventory(result.Items);
        OnInventory?.Invoke();
        OnGearChange?.Invoke(NetworkManager.LocalPlayerPeerId);
    }


    private void OnInventoryAdd(ResInventoryAdd response)
    {
        LocalInventory.AddItem(response.Id, response.Count);

        OnAddItem?.Invoke(response.Id, response.Count, LocalInventory.GetItemCount(response.Id));
    }
    
    private void OnInventoryRemove(ResInventoryRemove obj)
    {
        LocalInventory.RemoveItem(obj.Index, obj.Count);
        OnRemoveItem?.Invoke(obj.Index,LocalInventory.Items[obj.Index].Count);
    }


    private void OnRemoteInventory(ResRemoteInventory remoteInventory)
    {
        if (RemoteInventories.TryGetValue(remoteInventory.Id, out var inv))
            RemoteInventories[remoteInventory.Id] = remoteInventory.Items;
        else
            RemoteInventories.Add(remoteInventory.Id, remoteInventory.Items);

        OnGearChange?.Invoke(remoteInventory.Id);
    }

    public static ItemStack[] GetPlayerGear(int peerId)
    {
        if (peerId == NetworkManager.LocalPlayerPeerId)
            return new[]
            {
                LocalInventory.GetHelmet(),
                LocalInventory.GetArmor(),
                LocalInventory.GetHand()
            };
        return RemoteInventories[peerId];
    }

    public static void Sell(int index,short count)
    {
       NetworkManager.Instance.Send(new ReqSellItem()
       {
          Index = index,
           Count = count,
       });
    }
}