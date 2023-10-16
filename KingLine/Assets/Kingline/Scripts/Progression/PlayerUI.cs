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

    [SerializeField]
    private TMP_Text m_skillPointText;

    private Dictionary<string, SkillItemView> m_createdSkillItemViews = new();


    
    
    private void Start()
    {
        ProgressionNetworkController.Instance.OnLevelChange.AddListener(OnLevelChange);
        ProgressionNetworkController.Instance.OnSkillValueChanged.AddListener(OnSkillChanged);
    }

    private void OnSkillChanged(string skill, byte value)
    {
        m_createdSkillItemViews[skill].ValueText.text = value + "";
    }

    private void OnEnable()
    {
        ClearViews();
        CreateViews();
    }

    private void OnLevelChange(int level)
    {
    }

    private void CreateViews()
    {
        var playerSkill = ProgressionNetworkController.Instance.Skills;
        foreach (var skill in playerSkill)
        {
            CreateSkillView(skill);
        }

        var xp = ProgressionNetworkController.Instance.CurrentExp;
        var level = ProgressionNetworkController.Instance.Level;
        var neededXp = ProgressionNetworkController.Instance.MaxExp;
        m_xpText.text = $"Level {level}  ({xp}/{neededXp})";
        SetSkillViews();
    }

    public void SetSkillViews()
    {
        m_skillPointText.text = $"You have {ProgressionNetworkController.Instance.SkillPoint} Points";
        foreach (var m in m_createdSkillItemViews.Values)
        {
            m.IncrementButton.gameObject.SetActive(ProgressionNetworkController.Instance.SkillPoint > 0);
        }
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
        if (ProgressionNetworkController.Instance.SkillPoint <= 0)
            return;
        ProgressionNetworkController.Instance.SkillPoint--;
        ProgressionNetworkController.Instance.SendSkillIncrement(skill.Name);
        SetSkillViews();
    }

    private void ClearViews()
    {
        foreach (var s in m_createdSkillItemViews)
            Destroy(s.Value.gameObject);

        m_createdSkillItemViews.Clear();
    }
}