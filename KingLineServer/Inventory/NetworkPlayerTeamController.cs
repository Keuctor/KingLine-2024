using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Reflection.Metadata.Ecma335;

//Refugee
//Peasant Villager
//Spearman-Crossbowman
//Trained Spearman - Trained Crossbowman
//Veteran Spearman - Sharpshooter
//Sergeant

//Peasant
//Footman - Huntsman
//Trained Footman - Archer
//Warrior - Veteran Archer
//Veteran


public class NetworkPlayerTeamController : INetworkController
{
    public Dictionary<string, TeamMember[]> PlayerTeams = new Dictionary<string, TeamMember[]>();

    public void OnPeerConnected(NetPeer peer)
    {
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnStart()
    {
    }
    public void OnExit()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.RegisterNestedType(() =>
        {
            return new TeamMember();
        });
        processor.RegisterNestedType(() =>
        {
            return new Team();
        });
        processor.SubscribeReusable<ReqPlayerTeam, NetPeer>(OnRequestPlayerTeam);
    }

    private void OnRequestPlayerTeam(ReqPlayerTeam request, NetPeer peer)
    {
        var response = new ResPlayerTeam();
        var players = NetworkPlayerController.Players;

        response.Teams = new Team[players.Count];

        int i = 0;
        foreach (var player in players)
        {
            var team = GetPlayerTeam(player.Key.Id, player.Value.Token);
            response.Teams[i] = team;
            i++;
        }
        PackageSender.SendPacket(peer, response);
    }

    private Team GetPlayerTeam(int id, string token)
    {
        if (PlayerTeams.TryGetValue(token, out var members))
        {
            return new Team()
            {
                Id = id,
                Members = members
            };
        }

        PlayerTeams.Add(token, DefaultMembers);

        return new Team()
        {
            Id = id,
            Members = DefaultMembers,
        };
    }

    public void AddMember(string token, int id,short count) {
        var team = this.PlayerTeams[token];
        bool hasAny = false;
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].Id == id) {
                team[i].Count += count;
                hasAny = true;
            }
        }
        if (!hasAny)
        {
            var members = this.PlayerTeams[token].ToList();
            members.Add(new TeamMember()
            {
                Count = count,
                Id = id,
                Xp = 0,
            });
            this.PlayerTeams[token] = members.ToArray();
        }
    }

    public void GiveXpToTeam(string token, int xp)
    {
        var team = this.PlayerTeams[token];
        for (int i = 0; i < team.Length; i++)
        {
            team[i].Xp += xp;
        }
    }

    public TeamMember[] DefaultMembers = new TeamMember[4] {
        new TeamMember() { Id = 0, Count = 3 },
        new TeamMember() { Id = 1, Count = 6 },
        new TeamMember() { Id = 2, Count = 8 },
        new TeamMember() { Id = 3, Count = 9 }
    };
}