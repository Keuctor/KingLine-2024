using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

public class ResPlayerXp
{
    public int Xp { get; set; }
}
public class ResPlayerAddXp
{
    public int Xp { get; set; }
}


public class ReqPlayerXp { }


namespace KingLineServer.Inventory
{

    public class NetworkPlayerLevelController : INetworkController
    {
        static Dictionary<string, int> PlayerExperiences = new Dictionary<string, int>();

        public void OnExit()
        {
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

        public static void AddXp(NetPeer peer, int xp) {
            var player = NetworkPlayerController.Players[peer];
            PlayerExperiences[player.UniqueIdendifier] += xp;
            PackageSender.SendPacket(peer,new ResPlayerAddXp() {
                Xp = xp
            });
        }

        public void Subscribe(NetPacketProcessor processor)
        {
            processor.SubscribeReusable<ReqPlayerXp, NetPeer>(OnPlayerXpRequest);
        }

        private void OnPlayerXpRequest(ReqPlayerXp obj, NetPeer peer)
        {
            var player = NetworkPlayerController.Players[peer];
            var idendifier = player.UniqueIdendifier;

            var response = new ResPlayerXp();

            if (PlayerExperiences.TryGetValue(player.UniqueIdendifier, out var xp))
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
    }
}
