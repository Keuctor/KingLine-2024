using System;
using System.Collections.Generic;
using System.Text;


public class XPManager
{
    private static readonly int[] playerXpLevels;
    public static int[] TeamLevels;
    static XPManager()
    {
        playerXpLevels = new int[]
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
        TeamLevels = new int[]
        {
            500,
            1500,
            3500,
            6000,
            12000
        };
    }
    public static int GetTeamLevel(int xp) {
        int level = 1;
        for (int i = 0; i < TeamLevels.Length; i++)
        {
            if (xp >= TeamLevels[i])
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

    public static int GetNeededXpForNextTeamLevel(int xp)
    {
        return TeamLevels[GetTeamLevel(xp) - 1];
    }

    public static int GetLevel(int xp)
    {
        int level = 1;
        for (int i = 0; i < playerXpLevels.Length; i++)
        {
            if (xp >= playerXpLevels[i])
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

    public static int GetNeededXpForNextLevel(int xp)
    {
        return playerXpLevels[GetLevel(xp) - 1];
    }
}

