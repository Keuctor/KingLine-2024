using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;


public class XPManager
{
    private readonly int[] XpLevels;
    public XPManager()
    {
        XpLevels = new int[]
        {
            20,
            50,
            100,
            220,
            350,
            550,
            750,
            1000,
            1500,
            2250,
            3500,
            4250,
            6200,
            8100,
            9800,
            12000,
            15500,
            20000,
            22500,
            26000,
            32000,
            38000,
            44000,
            55000,
            67000,
            85000,
            101000,
            110000,
            155000,
            200000,
            256320,
            326425,
            484256,
            617025,
            834525,
        };
    }
    public int GetLevel(int xp)
    {
        int level = 1;
        for (int i = 0; i < XpLevels.Length; i++)
        {
            if (xp >= XpLevels[i])
            {
                level++;
            }
            else
            {
                break;
            }
        }
        return level;
    }

    public int GetNeededXpForNextLevel(int xp)
    {
        return XpLevels[GetLevel(xp) - 1];
    }
}


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
}
