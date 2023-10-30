using UnityEngine;
using UnityEngine.Serialization;

public class ProgressionController : MonoBehaviour
{
    [FormerlySerializedAs("m_leveLUpPopup")]
    [SerializeField]
    private LevelUpPopupView MLeveLUpPopupView;

    [SerializeField]
    private Transform m_levelUpContent;

    [SerializeField]
    private ProgressionNetworkController m_progressionNetworkController;
    
    private void Start()
    {
        m_progressionNetworkController.OnLevelChange.AddListener(OnLevelChange);
    }

    private void OnLevelChange(int arg0)
    {
        Instantiate(MLeveLUpPopupView, m_levelUpContent);
    }
}