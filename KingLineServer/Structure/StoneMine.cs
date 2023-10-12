using KingLineServer.Utils;
using LiteNetLib;


public class StoneMine : StructureBase
{
    public float StoneRatio = 1.2f;
    public int Id = 0;
    public StoneMine(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void OnPlayerGatherResource(NetPeer peer, Player player)
    {
        if (!IsPlayerNearby(player)) return;

    }
}
