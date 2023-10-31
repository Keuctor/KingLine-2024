using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ProgressionNetworkController : NetworkController
{
    private ProgressionNetworkController m_progressionController;

    [NonSerialized]
    public readonly UnityEvent<int> OnLevelChange = new();

    [NonSerialized]
    public readonly UnityEvent<string, byte> OnSkillValueChanged = new();

    [NonSerialized]
    public int CurrentExp;
    
    [NonSerialized]
    public readonly UnityEvent<int> OnSkillPointChanged = new();

    [NonSerialized]
    public int Level = -1;

    [NonSerialized]
    public int MaxExp;

    private int m_skillPoint;

    public int SkillPoint
    {
        get => m_skillPoint;
        set
        {
            if (m_skillPoint == value) return;
            m_skillPoint = value;
            OnSkillPointChanged?.Invoke(value);
        }
    }

   

    [NonSerialized]
    public Skill[] Skills;

    public TeamNetworkController TeamNetworkController;
    
    


    public override void OnPeerDisconnected(NetPeer peer)
    {
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqPlayerProgression());
        NetworkManager.Instance.Send(new ReqPlayerXp());
    }

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResPlayerProgression>(OnPlayerProgressionResponse);
        processor.SubscribeReusable<ResPlayerXp>(OnXpResponse);
        processor.SubscribeReusable<ResPlayerAddXp>(OnXpAdd);
        processor.SubscribeReusable<ResSkillValueChange>(OnSkillIncrement);
    }

    public override void OnExit()
    {
    }

    public override void OnStart()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
    }


    private void OnSkillIncrement(ResSkillValueChange obj)
    {
        foreach (var skill in Skills)
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


    private void OnXpAdd(ResPlayerAddXp obj)
    {
        CurrentExp += obj.Xp;
        var newLevel = XPManager.GetLevel(CurrentExp);
        if (Level != newLevel)
        {
            SkillPoint += newLevel - Level;
            Level = XPManager.GetLevel(CurrentExp);
            OnLevelChange?.Invoke(Level);
        }

        TeamNetworkController.GiveXp(obj.Xp);
    }

    private void OnXpResponse(ResPlayerXp obj)
    {
        CurrentExp = obj.Xp;
        MaxExp = XPManager.GetNeededXpForNextLevel(CurrentExp);

        Level = XPManager.GetLevel(obj.Xp);
        SkillPoint = Level;
        foreach (var n in Skills)
            SkillPoint -= n.Value - 1;
    }

    private void OnPlayerProgressionResponse(ResPlayerProgression obj)
    {
        Skills = obj.Skills;
    }

    public byte GetSkill(string name)
    {
        for (var i = 0; i < Skills.Length; i++)
            if (Skills[i].Name.Equals(name))
                return Skills[i].Value;

        return 0;
    }


    public void SendSkillIncrement(string skillName)
    {
        NetworkManager.Instance.Send(new ReqSkillIncrement
        {
            SkillName = skillName
        });
    }
}