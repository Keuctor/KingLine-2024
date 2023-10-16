using System;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


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
    public int Level { get; set; }
    public int NeededXpForNextLevel { get; set; }
}

public class ResPlayerAddXp
{
    public int Xp { get; set; }
}

public class ReqPlayerXp
{
}

public class ReqSkillIncrement
{
    public string SkillName { get; set; }
}

public class ResSkillValueChange
{
    public string SkillName { get; set; }
    public byte Value { get; set; }
}


public class ProgressionNetworkController : NetworkController<ProgressionNetworkController>
{
    public int Level = -1;
    public int MaxExp;
    public int CurrentExp;
    public Skill[] Skills;
    public PlayerUI PlayerUI;

    [NonSerialized]
    public readonly UnityEvent<int> OnLevelChange = new();
    [NonSerialized]
    public readonly UnityEvent<string,byte> OnSkillValueChanged = new();

    public int LevelPoint = 0;

    public override void SubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor.RegisterNestedType(() => new Skill());
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerProgression>(OnPlayerProgressionResponse);

        NetworkManager.Instance.NetPacketProcessor.SubscribeReusable<ResPlayerXp>(OnXpResponse);
        NetworkManager.Instance.NetPacketProcessor.SubscribeReusable<ResPlayerAddXp>(OnXpAdd);
        NetworkManager.Instance.NetPacketProcessor.SubscribeReusable<ResSkillValueChange>(OnSkillIncrement);
    }

    private void OnSkillIncrement(ResSkillValueChange obj)
    {
        foreach (var skill in Skills)
        {
            if (skill.Name.Equals(obj.SkillName))
            {
                if (skill.Value != obj.Value)
                {
                    skill.Value = obj.Value;
                    OnSkillValueChanged?.Invoke(skill.Name, skill.Value);
                }

                break;
            }
        }
    }


    private void OnXpAdd(ResPlayerAddXp obj)
    {
        this.CurrentExp += obj.Xp;
    }

    private void OnXpResponse(ResPlayerXp obj)
    {
        this.CurrentExp = obj.Xp;
        this.MaxExp = obj.NeededXpForNextLevel;
        if (this.Level != -1)
        {
            if (this.Level != obj.Level)
            {
                OnLevelChange?.Invoke(obj.Level);
            }
        }

        this.Level = obj.Level;
        LevelPoint = this.Level;
        foreach (var n in Skills)
            LevelPoint -= (n.Value - 1);
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

    public void SendSkillIncrement(string skillName)
    {
        NetworkManager.Instance.Send(new ReqSkillIncrement()
        {
            SkillName = skillName
        });
    }
}