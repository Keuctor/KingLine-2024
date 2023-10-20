using System.Collections.Generic;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.Events;

public class InventoryNetworkController : INetworkController
{
    public static NetworkInventory LocalInventory;

    public static Dictionary<int, ItemStack[]> RemoteInventories = new();
    public readonly UnityEvent<int, int> OnAddItem = new();

    public readonly UnityEvent<int> OnGearChange = new();
    public readonly UnityEvent OnInventory = new();

    public void OnPeerDisconnected(NetPeer peer)
    {
        RemoteInventories.Remove(peer.Id);
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
        processor.SubscribeReusable<ResRemoteInventory>(OnRemoteInventory);
    }


    public void OnExit()
    {
    }

    public void OnStart()
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
        OnAddItem?.Invoke(response.Id, response.Count);
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
}