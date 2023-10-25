using System;
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

    [SerializeField]
    private PrefabsSO m_prefabs;

    [SerializeField]
    private AudioManager m_audioManager;


    private void OnEnable()
    {
        if (m_progressionNetworkController == null)
            m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();

        ClearViews();
        CreateViews();
    }

    private void OnUpgradeTeam(bool upgraded)
    {
        if (upgraded)
        {
            ClearTroops();
            CreateTroops();
            m_audioManager.PlayOnce(SoundType.UPGRADE_TEAM, false, 1);
        }
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
        NetworkManager.Instance.GetController<PlayerTeamController>().OnUpgradeTeam.AddListener(OnUpgradeTeam);
        m_progressionNetworkController.OnLevelChange.AddListener(OnLevelChange);
        m_progressionNetworkController.OnSkillValueChanged.AddListener(OnSkillChanged);


        var xp = m_progressionNetworkController.CurrentExp;
        var level = m_progressionNetworkController.Level;
        var neededXp = m_progressionNetworkController.MaxExp;
        m_xpText.text = $"Level {level}  ({xp}/{neededXp})";

        var playerSkill = m_progressionNetworkController.Skills;
        foreach (var skill in playerSkill) CreateSkillView(skill);

        SetSkillViews();

        CreateTroops();
    }

    private void CreateTroops()
    {
        foreach (var troop in PlayerTeamController.LocalPlayerTeam)
            CreateTroopView(troop);
    }

    private void CreateTroopView(TeamMember troop)
    {
        var troopData = TroopRegistry.Troops[troop.Id];
        var troopView = Instantiate(m_troopItemView, m_troopItemViewContent);
        troopView.NameText.text = "x" + troop.Count + " " + troopData.Name;
        troopView.ValueText.text = "";
        troopView.Button.onClick.AddListener(() => { ShowTroop(troop); });
    }

    private void ShowTroop(TeamMember member)
    {
        var troopData = TroopRegistry.Troops[member.Id];
        var chView = Instantiate(m_prefabs.CharacterView);

        chView.Show(troopData.Name, troopData.Gear);
        chView.SetXp(member.Xp,troopData.UpgradeXp);

        if (member.Xp >= troopData.UpgradeXp && troopData.NextTroopId != -1)
        {
            chView.SetUpgrade(troopData.UpgradePrice);
            chView.OnUpgradeClicked.AddListener(() =>
            {
                var teamController = NetworkManager.Instance.GetController<PlayerTeamController>();
                teamController.UpgradeTeam(member.Id);
                Destroy(chView.gameObject);
            });
        }
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
        NetworkManager.Instance.GetController<PlayerTeamController>().OnUpgradeTeam.RemoveListener(OnUpgradeTeam);
        m_progressionNetworkController.OnLevelChange.RemoveListener(OnLevelChange);
        m_progressionNetworkController.OnSkillValueChanged.RemoveListener(OnSkillChanged);


        ClearTroops();

        foreach (var s in m_createdSkillItemViews)
            Destroy(s.Value.gameObject);

        m_createdSkillItemViews.Clear();
    }

    private void ClearTroops()
    {
        foreach (Transform t in m_troopItemViewContent)
            Destroy(t.gameObject);
    }
}