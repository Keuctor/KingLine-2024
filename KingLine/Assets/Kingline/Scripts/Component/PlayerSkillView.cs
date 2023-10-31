using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField]
    private ProgressionNetworkController m_progressionController;
    
    [Header("Prefabs")]
    [SerializeField]
    private NameValueButtonView m_skillItemViewTemplate;
    
    [Header("Parents")]
    [SerializeField]
    private Transform m_skillItemViewParent;
    
    public bool IsModifiable = true;
    
    private readonly Dictionary<string, NameValueButtonView> m_createdSkillItemViews = new();

    private void OnDisable()
    {
        foreach(var v in m_createdSkillItemViews)
            Destroy(v.Value.gameObject);
        
        m_createdSkillItemViews.Clear();
        
        m_progressionController.OnSkillValueChanged.RemoveListener(OnSkillChanged);
    }

    private void OnEnable()
    {
        m_progressionController.OnSkillValueChanged.AddListener(OnSkillChanged);

        var playerSkill = m_progressionController.Skills;
        
        foreach (var skill in playerSkill)
        {
            var skillView = Instantiate(m_skillItemViewTemplate, m_skillItemViewParent);
            skillView.NameText.text = skill.Name;
            skillView.ValueText.text = skill.Value + "";
            skillView.Button.gameObject.SetActive(IsModifiable);
            if (IsModifiable)
            {
                skillView.Button.onClick.AddListener(() => { OnIncrementSkillPointClicked(skill); });
            }
            m_createdSkillItemViews.Add(skill.Name, skillView);
        }
    }
    
    private void OnIncrementSkillPointClicked(Skill skill)
    {
        if (m_progressionController.SkillPoint <= 0)
            return;
        m_progressionController.SkillPoint--;
        m_progressionController.SendSkillIncrement(skill.Name);
        foreach (var m in m_createdSkillItemViews.Values)
            m.Button.gameObject.SetActive(m_progressionController.SkillPoint > 0);
    }
    
    private void OnSkillChanged(string skill, byte value)
    {
        m_createdSkillItemViews[skill].ValueText.text = value + "";
    }
}
