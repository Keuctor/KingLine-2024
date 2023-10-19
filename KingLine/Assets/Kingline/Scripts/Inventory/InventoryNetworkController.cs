using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.Events;

public class InventoryNetworkController : INetworkController
{
    public static NetworkInventory Inventory;

    public readonly UnityEvent OnGearChange = new();
    public readonly UnityEvent OnInventory = new();
    public readonly UnityEvent<int, int> OnAddItem = new();

    public static async Task<NetworkInventory> GetInventoryAsync()
    {
        while (Inventory == null)
        {
            await Task.Yield();
        }
        return Inventory;
    }

    private bool IsGearIndex(int index)
    {
        return index is NetworkInventory.HELMET_SLOT_INDEX or NetworkInventory.ARMOR_SLOT_INDEX
            or NetworkInventory.HAND_SLOT_INDEX;
    }

    private void OnInventoryMove(ResInventoryMove inventoryMove)
    {
        Inventory.MoveItem(inventoryMove.FromIndex, inventoryMove.ToIndex);
        if (IsGearIndex(inventoryMove.FromIndex) || IsGearIndex(inventoryMove.ToIndex))
        {
            OnGearChange?.Invoke();
        }
    }


    private void OnInventoryResponse(ResInventory result)
    {
        Inventory = new NetworkInventory(result.Items);
        OnInventory?.Invoke();
    }


    private void OnInventoryAdd(ResInventoryAdd response)
    {
        Inventory.AddItem(response.Id, response.Count);
        OnAddItem?.Invoke(response.Id, response.Count);
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