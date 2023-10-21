using UnityEngine;

public class ProgressionController : MonoBehaviour
{
    [SerializeField]
    private LevelUpPopup m_leveLUpPopup;

    [SerializeField]
    private Transform m_levelUpContent;

    private ProgressionNetworkController m_progressionNetworkController;

    private void Start()
    {
        m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();
        m_progressionNetworkController.OnLevelChange.AddListener(OnLevelChange);
    }

    private void OnLevelChange(int arg0)
    {
        Instantiate(m_leveLUpPopup, m_levelUpContent);
    }
}