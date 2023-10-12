using System;



    public abstract class StructureBase
    {
        public float x;
        public float y;
        public bool IsPlayerNearby(Player player)
        {
            if (Math.Abs(player.x - x) < 4f && Math.Abs(player.y - y) < 4)
            {
                return true;
            }
            return false;
        }
    }
