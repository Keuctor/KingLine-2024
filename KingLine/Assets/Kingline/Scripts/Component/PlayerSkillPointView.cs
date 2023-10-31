using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSkillPointView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField]
    private ProgressionNetworkController m_progressionController;

    [SerializeField]
    private TMP_Text m_skillPointText;
    
    [Header("Settings")]
    [SerializeField]
    private string m_formatting = "You have {0} skill points";
    
    private void OnEnable()
    {
        m_progressionController.OnLevelChange.AddListener(OnLevelChanged);
        Display();
    }

    private void Display()
    {
        m_skillPointText.text = string.Format(m_formatting, m_progressionController.SkillPoint);
    }

    private void OnLevelChanged(int arg0)
    {
        Display();
    }

    private void OnDisable()
    {
        m_progressionController.OnLevelChange.RemoveListener(OnLevelChanged);
    }
}
