using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class TeamNetworkController : NetworkController
{
    public static Dictionary<int, TeamMember[]> PlayerTeams = new();

    public override void OnPeerDisconnected(NetPeer peer)
    {
        //on disconnected from server
        PlayerTeams.Clear();
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public TeamMember[] LocalPlayerTeam
        => PlayerTeams[NetworkManager.LocalPlayerPeerId];

    public readonly UnityEvent<bool> OnUpgradeTeam = new();
    public readonly UnityEvent<int,int,short> OnVolunteersResponse = new();

    public override void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqPlayerTeam());
    }

    public void GiveXp(int xp)
    {
        foreach (var member in LocalPlayerTeam)
            member.Xp += xp;
    }

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResPlayerTeam>(OnPlayerTeamResponse);
        processor.SubscribeReusable<ResUpdatePlayerTeam>(OnUpdatePlayerTeamResponse);
        processor.SubscribeReusable<ResUpgradeTeam>(OnUpgradeTeamResponse);
        processor.SubscribeReusable<ResVolunteers>(OnResponseVolunteers);
    }

    private void OnResponseVolunteers(ResVolunteers obj)
    {
        OnVolunteersResponse?.Invoke(obj.StructureId,obj.TroopId,obj.Count);
    }

    private void OnUpgradeTeamResponse(ResUpgradeTeam response)
    {
        OnUpgradeTeam?.Invoke(response.Success);
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
    public void UpgradeTeam(int memberId)
    {
        NetworkManager.Instance.Send(new ReqUpgradeTeam()
        {
            MemberId = memberId,
        });
    }
    
    public void RequestVolunteers(int structureId)
    {
        NetworkManager.Instance.Send(new ReqVolunteers()
        {
            StructureId = structureId
        });
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


   
}