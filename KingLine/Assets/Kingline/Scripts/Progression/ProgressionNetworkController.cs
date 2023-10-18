using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;


public class ProgressionNetworkController : INetworkController
{
    public int Level = -1;
    public int MaxExp;
    public int CurrentExp;
    public Skill[] Skills;

    [NonSerialized]
    public readonly UnityEvent<int> OnLevelChange = new();

    [NonSerialized]
    public readonly UnityEvent<string, byte> OnSkillValueChanged = new();

    [NonSerialized]
    public int SkillPoint = 0;


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
        SkillPoint = this.Level;
        foreach (var n in Skills)
            SkillPoint -= (n.Value - 1);
    }

    private void OnPlayerProgressionResponse(ResPlayerProgression obj)
    {
        Skills = obj.Skills;
    }

    public byte GetSkill(string name)
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i].Name.Equals(name))
                return Skills[i].Value;
        }

        return 0;
    }


    public void SendSkillIncrement(string skillName)
    {
        NetworkManager.Instance.Send(new ReqSkillIncrement()
        {
            SkillName = skillName
        });
    }


    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqPlayerProgression());
        NetworkManager.Instance.Send(new ReqPlayerXp());
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResPlayerProgression>(OnPlayerProgressionResponse);
        processor.SubscribeReusable<ResPlayerXp>(OnXpResponse);
        processor.SubscribeReusable<ResPlayerAddXp>(OnXpAdd);
        processor.SubscribeReusable<ResSkillValueChange>(OnSkillIncrement);
    }

    public void OnExit()
    {
    }

    public void OnStart()
    {
    }
}