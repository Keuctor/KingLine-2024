using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class PlayerTeamController : INetworkController
{
    public static Dictionary<int, TeamMember[]> PlayerTeams = new();

    public void OnPeerDisconnected(NetPeer peer)
    {
        //on disconnected from server
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public static TeamMember[] LocalPlayerTeam
        => PlayerTeams[NetworkManager.LocalPlayerPeerId];

    public void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqPlayerTeam());
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResPlayerTeam>(OnPlayerTeamResponse);
        processor.SubscribeReusable<ResUpdatePlayerTeam>(OnUpdatePlayerTeamResponse);
    }

    private void OnUpdatePlayerTeamResponse(ResUpdatePlayerTeam response)
    {
        PlayerTeams[response.Team.Id] = response.Team.Members;
    }

    private void OnPlayerTeamResponse(ResPlayerTeam teams)
    {
        foreach (var team in teams.Teams)
        {
            PlayerTeams.Add(team.Id, team.Members);
        }
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
}