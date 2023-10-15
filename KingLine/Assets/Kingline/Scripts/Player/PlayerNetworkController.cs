using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerNetworkController : NetworkController<PlayerNetworkController>
{
    public readonly Dictionary<int, Player> Players = new();

    public bool Completed;

    public override void SubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayers>(OnPlayersResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerPosition>(OnUpdatePlayerPositionResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerMove>(OnPlayerTargetChangeResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerJoin>(OnPlayerJoinedResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerLeave>(OnPlayerLeaveResponse);
    }

    public override void HandleRequest()
    {
        NetworkManager.Instance.Send(new ReqPlayers());
    }

    public override void UnSubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayers>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerPosition>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerMove>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerJoin>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerLeave>();
    }

    public override void OnDisconnectedFromServer()
    {
        Players.Clear();
    }


    [NonSerialized]
    public UnityEvent OnPlayerListRefresh = new();
    [NonSerialized]
    public UnityEvent<int> OnPlayerJoin = new();
    [NonSerialized]
    public UnityEvent<int> OnPlayerLeave = new();
    
    #region NETWORK_RESPONSE

    private void OnPlayersResponse(ResPlayers res)
    {
        for (var i = 0; i < res.Players.Length; i++)
        {
            Players.Add(res.Players[i].Id, res.Players[i]);
        }
        Completed = true;
        OnPlayerListRefresh?.Invoke();
    }

    private void OnPlayerTargetChangeResponse(ResPlayerMove target)
    {
        var p = Players[target.Id];
        if (p.IsLocalPlayer) return;

        p.targetX = target.x;
        p.targetY = target.y;
    }

    private void OnUpdatePlayerPositionResponse(ResPlayerPosition target)
    {
        var p = Players[target.Id];
        p.x = target.x;
        p.y = target.y;
    }

    private void OnPlayerJoinedResponse(ResPlayerJoin resPlayer)
    {
        Players.Add(resPlayer.Player.Id, resPlayer.Player);
        OnPlayerJoin?.Invoke(resPlayer.Player.Id);
    }

    private void OnPlayerLeaveResponse(ResPlayerLeave resPlayer)
    {
        for (var i = 0; i < Players.Count; i++)
            if (Players[i].Id == resPlayer.Player.Id)
            {
                Players.Remove(resPlayer.Player.Id);
                OnPlayerLeave?.Invoke(resPlayer.Player.Id);
                break;
            }
    }

    #endregion

    public Player GetPlayer(int i)
    {
        return Players[i];
    }
}