using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Transform m_skillItemViewContent;

    [SerializeField]
    private NameValueButtonView m_skillItemView;

    [SerializeField]
    private NameValueButtonView m_troopItemView;

    [SerializeField]
    private Transform m_troopItemViewContent;

    [SerializeField]
    private TMP_Text m_xpText;

    [SerializeField]
    private TMP_Text m_skillPointText;

    private readonly Dictionary<string, NameValueButtonView> m_createdSkillItemViews = new();

    private ProgressionNetworkController m_progressionNetworkController;

    private void Start()
    {
        m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();
        m_progressionNetworkController.OnLevelChange.AddListener(OnLevelChange);
        m_progressionNetworkController.OnSkillValueChanged.AddListener(OnSkillChanged);


        ClearViews();
        CreateViews();
    }

    private void OnSkillChanged(string skill, byte value)
    {
        m_createdSkillItemViews[skill].ValueText.text = value + "";
    }

    private void OnLevelChange(int level)
    {
    }

    private void CreateViews()
    {
        var playerSkill = m_progressionNetworkController.Skills;
        foreach (var skill in playerSkill) CreateSkillView(skill);

        var xp = m_progressionNetworkController.CurrentExp;
        var level = m_progressionNetworkController.Level;
        var neededXp = m_progressionNetworkController.MaxExp;
        m_xpText.text = $"Level {level}  ({xp}/{neededXp})";
        SetSkillViews();

        foreach (var troop in PlayerTeamController.LocalPlayerTeam)
            CreateTroopView(troop);
    }

    private void CreateTroopView(TeamMember troop)
    {
        var troopData = TroopRegistry.Troops[troop.Id];
        var troopView = Instantiate(m_troopItemView, m_troopItemViewContent);
        troopView.NameText.text = "x"+troop.Count+" "+troopData.Name;
        troopView.ValueText.text = "";
        troopView.Button.onClick.AddListener(() =>
        {
            ShowTroop(troop.Id);
        });
    }

    private void ShowTroop(int id)
    {
        var troopData = TroopRegistry.Troops[id];
        //show it 
    }

    public void SetSkillViews()
    {
        m_skillPointText.text = $"You have {m_progressionNetworkController.SkillPoint} Points";
        foreach (var m in m_createdSkillItemViews.Values)
            m.Button.gameObject.SetActive(m_progressionNetworkController.SkillPoint > 0);
    }

    public void CreateSkillView(Skill skill)
    {
        var skillView = Instantiate(m_skillItemView, m_skillItemViewContent);
        skillView.NameText.text = skill.Name;
        skillView.ValueText.text = skill.Value + "";
        skillView.Button.onClick.AddListener(() => { OnIncrementSkillPointClicked(skill); });
        m_createdSkillItemViews.Add(skill.Name, skillView);
    }

    private void OnIncrementSkillPointClicked(Skill skill)
    {
        if (m_progressionNetworkController.SkillPoint <= 0)
            return;
        m_progressionNetworkController.SkillPoint--;
        m_progressionNetworkController.SendSkillIncrement(skill.Name);
        SetSkillViews();
    }

    private void ClearViews()
    {
        foreach (var s in m_createdSkillItemViews)
            Destroy(s.Value.gameObject);

        m_createdSkillItemViews.Clear();
    }
}