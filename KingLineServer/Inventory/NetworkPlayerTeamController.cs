using LiteNetLib;
using LiteNetLib.Utils;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

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
    public static Dictionary<string, TeamMember[]> PlayerTeams = new Dictionary<string, TeamMember[]>();

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
        processor.SubscribeReusable<ReqUpgradeTeam, NetPeer>(OnRequestUpgradeTeam);
    }

    private void OnRequestUpgradeTeam(ReqUpgradeTeam request, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var troop = TroopRegistry.Troops[request.MemberId];

        var team = PlayerTeams[player.Token];
        var success = false;
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].Id == request.MemberId)
            {
                if (troop.NextTroopId != -1)
                {
                    if (team[i].Xp >= troop.UpgradeXp)
                    {
                        team[i].Xp -= troop.UpgradeXp;

                        RemoveMember(player.Token, team[i].Id);
                        AddMember(player.Token, troop.NextTroopId, 1);

                        success = true;
                        PackageSender.SendPacket(peer, new ResUpdatePlayerTeam()
                        {
                            Team = new Team()
                            {
                                Id = player.Id,
                                Members = PlayerTeams[player.Token]
                            }
                        });
                    }
                }
                break;
            }
        }
        PackageSender.SendPacket(peer, new ResUpgradeTeam()
        {
            Success = success
        });
    }
    public static void RemoveMember(string user, int id)
    {
        var team = PlayerTeams[user];
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].Id == id)
            {
                team[i].Count--;
                Console.WriteLine(team[i].Count);
                if (team[i].Count <= 0)
                {
                    var list = team.ToList();
                    list.Remove(team[i]);
                    PlayerTeams[user] = list.ToArray();
                }
                break;
            }
        }
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

    private static Team GetPlayerTeam(int id, string token)
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

    public static void AddMember(string token, int id, short count)
    {
        var team = PlayerTeams[token];
        bool hasAny = false;
        for (int i = 0; i < team.Length; i++)
        {
            if (team[i].Id == id)
            {
                team[i].Count += count;
                hasAny = true;
            }
        }
        if (!hasAny)
        {
            var members = PlayerTeams[token].ToList();
            members.Add(new TeamMember()
            {
                Count = count,
                Id = id,
                Xp = 0,
            });
            PlayerTeams[token] = members.ToArray();
        }
    }

    public static void GiveXpToTeam(string token, int xp)
    {
        var team = PlayerTeams[token];
        for (int i = 0; i < team.Length; i++)
        {
            team[i].Xp += xp;
        }
    }

    public static TeamMember[] DefaultMembers
    {
        get
        {
            return new TeamMember[1]
            {
                new TeamMember() { Id = 0, Count = 5 }
            };
         }
    }
}