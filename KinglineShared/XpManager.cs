using System;
using System.Collections.Generic;
using System.Text;


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

