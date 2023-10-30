using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIView : MonoBehaviour
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


    [SerializeField]
    private PrefabsSO m_prefabs;

    [SerializeField]
    private AudioManager m_audioManager;

    public ProgressionNetworkController ProgressionNetworkController;
    public TeamNetworkController TeamNetworkController;


    private void OnEnable()
    {
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
        TeamNetworkController.OnUpgradeTeam.AddListener(OnUpgradeTeam);
        ProgressionNetworkController.OnLevelChange.AddListener(OnLevelChange);
        ProgressionNetworkController.OnSkillValueChanged.AddListener(OnSkillChanged);


        var xp = ProgressionNetworkController.CurrentExp;
        var level = ProgressionNetworkController.Level;
        var neededXp = ProgressionNetworkController.MaxExp;
        m_xpText.text = $"Level {level}  ({xp}/{neededXp})";

        var playerSkill = ProgressionNetworkController.Skills;
        foreach (var skill in playerSkill) CreateSkillView(skill);

        SetSkillViews();

        CreateTroops();
    }

    private void CreateTroops()
    {
        foreach (var troop in TeamNetworkController.LocalPlayerTeam)
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
                TeamNetworkController.UpgradeTeam(member.Id);
                Destroy(chView.gameObject);
            });
        }
    }

    public void SetSkillViews()
    {
        m_skillPointText.text = $"You have {ProgressionNetworkController.SkillPoint} Points";
        foreach (var m in m_createdSkillItemViews.Values)
            m.Button.gameObject.SetActive(ProgressionNetworkController.SkillPoint > 0);
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
        if (ProgressionNetworkController.SkillPoint <= 0)
            return;
        ProgressionNetworkController.SkillPoint--;
        ProgressionNetworkController.SendSkillIncrement(skill.Name);
        SetSkillViews();
    }

    private void ClearViews()
    {
        TeamNetworkController.OnUpgradeTeam.RemoveListener(OnUpgradeTeam);
        ProgressionNetworkController.OnLevelChange.RemoveListener(OnLevelChange);
        ProgressionNetworkController.OnSkillValueChanged.RemoveListener(OnSkillChanged);


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