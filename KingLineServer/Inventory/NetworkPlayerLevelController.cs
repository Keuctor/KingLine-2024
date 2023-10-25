using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;


public class NetworkPlayerLevelController : INetworkController
{
    public static Dictionary<string, int> PlayerExperiences = new Dictionary<string, int>();


    public void OnExit()
    {
    }

    public static int GetPlayerLevel(string idendifier)
    {
        return XPManager.GetLevel(PlayerExperiences[idendifier]);
    }

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

    public static void AddXp(NetPeer peer, int xp)
    {
        xp *= KingLineServer.Multiplier;
        var player = NetworkPlayerController.Players[peer];
        PlayerExperiences[player.Token] += xp;
        NetworkPlayerTeamController.GiveXpToTeam(player.Token, xp);
        PackageSender.SendPacket(peer, new ResPlayerAddXp()
        {
            Xp = xp,
        });
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ReqPlayerXp, NetPeer>(OnPlayerXpRequest);
    }

    private void OnPlayerXpRequest(ReqPlayerXp obj, NetPeer peer)
    {
        var player = NetworkPlayerController.Players[peer];
        var idendifier = player.Token;

        var response = new ResPlayerXp();

        if (PlayerExperiences.TryGetValue(player.Token, out var xp))
        {
            response.Xp = xp;
        }
        else
        {
            PlayerExperiences.Add(idendifier, 0);
            response.Xp = 0;
        }
        PackageSender.SendPacket(peer, response);
    }

    public void OnUpdate(float deltaTime)
    {
    }
}
