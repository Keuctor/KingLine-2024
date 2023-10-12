using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StructureInfoUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_content;

    [SerializeField]
    private TMP_Text m_titleText;

    [SerializeField]
    private TMP_Text m_descriptionText;

    [SerializeField]
    private Image m_placeIconImage;

    [SerializeField]
    private Button[] m_buttons;

    [NonSerialized]
    public readonly UnityEvent<int> OnClicked = new();

    private StructureBehaviour m_structure;

    private void Start()
    {
        for (var i = 0; i < m_buttons.Length; i++)
        {
            var button = m_buttons[i];
            var index = i;
            button.onClick.AddListener(() => { OnClicked?.Invoke(index); });
        }

        m_content.anchoredPosition = new Vector2(0, -1300f);
        StartCoroutine(AfterFrame());
    }

    private void OnDestroy()
    {
        m_structure.Selection.gameObject.SetActive(false);
    }

    public void SetView(StructureBehaviour structureBehaviour)
    {
        if (m_structure != null) m_structure.Selection.gameObject.SetActive(false);
        m_structure = structureBehaviour;
        m_titleText.text = structureBehaviour.Name;
        m_descriptionText.text = structureBehaviour.Description;
        m_placeIconImage.sprite = structureBehaviour.Icon;
        m_structure.Selection.gameObject.SetActive(true);

        m_structure.Selection.color = new Color(1f, 1f, 1f, 0);
        m_structure.Selection.DOFade(1, 0.25f);
        m_structure.Selection.transform.localScale = Vector3.zero;
        m_structure.Selection.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.25f);
    }

    private IEnumerator AfterFrame()
    {
        yield return new WaitForEndOfFrame();
        m_content.anchoredPosition = new Vector2(0, -m_content.sizeDelta.y);
        m_content.DOAnchorPos(Vector2.zero, 0.2f);
    }
}