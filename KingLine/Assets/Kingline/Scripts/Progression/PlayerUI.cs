using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Transform m_skillItemViewContent;

    [SerializeField]
    private SkillItemView m_skillItemView;

    [SerializeField]
    private TMP_Text m_xpText;

    private Dictionary<string, SkillItemView> m_createdSkillItemViews = new();

    private void OnEnable()
    {
        ClearViews();
        CreateViews();
    }

    private void CreateViews()
    {
        var playerSkill = ProgressionNetworkController.Instance.Skills;
        foreach (var skill in playerSkill)
        {
            CreateSkillView(skill);
        }

        var xpManager = ProgressionNetworkController.Instance.XpManager;
        var xp = ProgressionNetworkController.Instance.Xp;
        var level = xpManager.getLevel(ProgressionNetworkController.Instance.Xp);
        var neededXp = xpManager.getNeededXpForNextLevel(xp);
        m_xpText.text = $"Level {level}({xp}/{neededXp})";
    }

    public void CreateSkillView(Skill skill)
    {
        var skillView = Instantiate(m_skillItemView, m_skillItemViewContent);
        skillView.NameText.text = skill.Name;
        skillView.ValueText.text = skill.Value + "";
        skillView.IncrementButton.onClick.AddListener(() => { OnIncrementSkillPointClicked(skill); });
        m_createdSkillItemViews.Add(skill.Name, skillView);
    }

    private void OnIncrementSkillPointClicked(Skill skill)
    {
        Debug.Log(skill.Name);

        skill.Value += 1;
        m_createdSkillItemViews[skill.Name].ValueText.text = skill.Value + "";
    }

    private void ClearViews()
    {
        foreach (var s in m_createdSkillItemViews)
            Destroy(s.Value.gameObject);

        m_createdSkillItemViews.Clear();
    }
}