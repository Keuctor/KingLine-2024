using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelUpPopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_titleText;

    [SerializeField]
    private TMP_Text m_levelText;

    [SerializeField]
    private TMP_Text m_unspentSkillPointText;

    [SerializeField]
    private RectTransform m_rectTransform;

    private void Start()
    {
        AudioManager.Instance.PlayOnce(SoundType.LEVEL_UP,true,0.4f);
        this.m_levelText.text = $"LEVEL {ProgressionNetworkController.Instance.Level}";
        this.m_unspentSkillPointText.text =
            $"You have {ProgressionNetworkController.Instance.SkillPoint} unspent skill points";


        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        yield return new WaitForEndOfFrame();
        m_rectTransform.anchoredPosition = new Vector2(0, m_rectTransform.sizeDelta.y);
        m_rectTransform.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(5f);
        m_rectTransform.DOAnchorPos(new Vector2(0, m_rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.InBounce);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}