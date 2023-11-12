using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;


public class NetworkPlayerLevelController : INetworkController
{
    public static Dictionary<string, int> PlayerExperiences = new Dictionary<string, int>();


    public void OnExit()
    {
    }

    public static int GetPlayerLevel(string token)
    {
        return XPManager.GetLevel(PlayerExperiences[token]);
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
        xp *= KingLine.Multiplier;
        var token = KingLine.GetPlayerToken(peer.Id);
        PlayerExperiences[token] += xp;
        NetworkPlayerTeamController.GiveXpToTeam(token, xp);
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
        var token = KingLine.GetPlayerToken(peer.Id);
        var response = new ResPlayerXp();

        if (PlayerExperiences.TryGetValue(token, out var xp))
        {
            response.Xp = xp;
        }
        else
        {
            PlayerExperiences.Add(token, 0);
            response.Xp = 0;
        }
        PackageSender.SendPacket(peer, response);
    }

    public void OnUpdate(float deltaTime)
    {
    }
}
