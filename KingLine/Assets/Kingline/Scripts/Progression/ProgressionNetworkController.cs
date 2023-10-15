using System;
using LiteNetLib.Utils;
using UnityEngine;


[Serializable]
public class Skill : INetSerializable
{
    public string Name { get; set; }
    public byte Value { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put(Value);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        Value = reader.GetByte();
    }
}


public class ReqPlayerProgression
{
}

public class ResPlayerProgression
{
    public Skill[] Skills { get; set; }
}

public class ResPlayerXp
{

    public int Xp { get; set; }
}
public class ResPlayerAddXp
{
    public int Xp { get; set; }
}

public class ReqPlayerXp { }

public class XPManager
{
    public int[] XpLevels;
    private int hardLimit = 30;
    
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

    public int getLevel(int xp)
    {
        int level = 1;
        for (int i = 0; i < XpLevels.Length; i++)
        {
            if (xp>=XpLevels[i])
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

    public int getNeededXpForNextLevel(int xp)
    {
        return XpLevels[getLevel(xp)-1];
    }
}


public class ProgressionNetworkController : NetworkController<ProgressionNetworkController>
{
    public int Xp;
    public Skill[] Skills;
    public PlayerUI PlayerUI;
    public XPManager XpManager;

    
    public override void SubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor.RegisterNestedType(() => new Skill());
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerProgression>(OnPlayerProgressionResponse);
        
        NetworkManager.Instance.NetPacketProcessor.SubscribeReusable<ResPlayerXp>(OnXpResponse);
        NetworkManager.Instance.NetPacketProcessor.SubscribeReusable<ResPlayerAddXp>(OnXpAdd);
        XpManager = new XPManager();
    }
   
    private void OnXpAdd(ResPlayerAddXp obj)
    {
        this.Xp += obj.Xp;
    }

    private void OnXpResponse(ResPlayerXp obj)
    {
        this.Xp = obj.Xp;
    }

    private void OnPlayerProgressionResponse(ResPlayerProgression obj)
    {
        Skills = obj.Skills;
    }

    public override void HandleRequest()
    {
        NetworkManager.Instance.Send(new ReqPlayerProgression());
        NetworkManager.Instance.Send(new ReqPlayerXp());
    }

    public override void UnSubscribeResponse()
    {
    }

    public void OpenProgressionMenu()
    {
        PlayerUI.gameObject.SetActive(!PlayerUI.gameObject.activeInHierarchy);
    }

    public override void OnDisconnectedFromServer()
    {
    }
}