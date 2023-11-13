using UnityEngine;

public class PlayerTeamView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField]
    private TeamNetworkController m_teamController;

    [SerializeField]
    private AudioManager m_audioManager;

    [Header("Prefabs")]
    [SerializeField]
    private NameValueButtonView m_memberViewTemplate;

    [SerializeField]
    private CharacterView m_characterViewTemplate;

    [Header("Parents")]
    [SerializeField]
    private Transform m_memberViewParent;

    public bool IsUpgradable = true;

    private void OnEnable()
    {
        m_teamController.OnUpgradeTeam.AddListener(OnUpgradeTeam);
        CreateTeamUI();
    }

    private void CreateTeamUI()
    {
        foreach (var member in m_teamController.LocalPlayerTeam)
        {
            var troop = TroopRegistry.GetTroop(member.Id);
            var memberView = Instantiate(m_memberViewTemplate, m_memberViewParent);
            memberView.ValueText.text = "";
            memberView.NameText.text = $"x{member.Count} {troop.Name}";
            memberView.Button.gameObject.SetActive(IsUpgradable);
            if (IsUpgradable)
            {
                memberView.Button.onClick.AddListener(() => { ShowTroop(member); });
            }
        }
    }

    private void ShowTroop(TeamMember member)
    {
        var troopData = TroopRegistry.Troops[member.Id];
        var chView = Instantiate(m_characterViewTemplate);

        chView.Show(troopData.Name, troopData.Gear);
        chView.SetXp(member.Xp, troopData.UpgradeXp);

        if (member.Xp >= troopData.UpgradeXp && troopData.NextTroopId != -1)
        {
            chView.SetUpgrade(troopData.UpgradePrice);
            chView.OnUpgradeClicked.AddListener(() =>
            {
                m_teamController.UpgradeTeam(member.Id);
                Destroy(chView.gameObject);
            });
        }
    }

    private void OnUpgradeTeam(bool upgraded)
    {
        ClearTeams();
        CreateTeamUI();
        if (upgraded)
        {
            m_audioManager.PlayOnce(SoundType.UPGRADE_TEAM, false, 1);
        }
    }

    private void OnDisable()
    {
        m_teamController.OnUpgradeTeam.RemoveListener(OnUpgradeTeam);
        ClearTeams();
    }

    private void ClearTeams()
    {
        for (int i = 0; i < m_memberViewParent.childCount; i++)
            Destroy(m_memberViewParent.GetChild(i).gameObject);
    }
}