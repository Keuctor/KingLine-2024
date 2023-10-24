using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;


public class NetworkPlayerLevelController : INetworkController
{
    public static Dictionary<string, int> PlayerExperiences = new Dictionary<string, int>();
    static XPManager xPManager = new XPManager();

    public void OnExit()
    {
    }

    public static int GetPlayerLevel(string idendifier)
    {
        return xPManager.GetLevel(PlayerExperiences[idendifier]);
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
        var player = NetworkPlayerController.Players[peer];
        var currentLevel = xPManager.GetLevel(PlayerExperiences[player.Token]);
        PlayerExperiences[player.Token] += xp;

        var afterXp = PlayerExperiences[player.Token];
        var afterLevel = xPManager.GetLevel(afterXp);
        if (currentLevel != afterLevel)
        {
            PackageSender.SendPacket(peer, new ResPlayerXp()
            {
                Level = afterLevel,
                NeededXpForNextLevel = xPManager.GetNeededXpForNextLevel(afterXp),
                Xp = afterXp
            });
        }
        else
        {
            PackageSender.SendPacket(peer, new ResPlayerAddXp()
            {
                Xp = xp
            });
        }
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
            response.Level = xPManager.GetLevel(xp);
            response.NeededXpForNextLevel = xPManager.GetNeededXpForNextLevel(xp);
        }
        else
        {
            PlayerExperiences.Add(idendifier, 0);
            response.Xp = 0;
            response.Level = 1;
            response.NeededXpForNextLevel = xPManager.GetNeededXpForNextLevel(0);
        }
        PackageSender.SendPacket(peer, response);
    }

    public void OnUpdate(float deltaTime)
    {
    }
}
