using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLevelView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text levelText;

    [Header("Dependency")]
    [SerializeField]
    private ProgressionNetworkController m_progressionController;
    private void OnEnable()
    {
        levelText.text =
            $"Level {m_progressionController.Level}({m_progressionController.CurrentExp}/{m_progressionController.MaxExp})";
    }
}
