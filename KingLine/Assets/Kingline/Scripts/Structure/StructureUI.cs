using System;
using Kingline.Scripts.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StructureUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_titleText;

    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private Button m_buttonOptionTemplate;

    [SerializeField]
    private Transform m_buttonOptionContent;

    [NonSerialized]
    public UnityEvent<int> OnResult = new();

    public void SetContext(StructureInfo structureInfo)
    {
        m_titleText.text = structureInfo.EnterDescription;
        m_icon.sprite = structureInfo.Icon;

        var options = structureInfo.Options;
        for (var i = 0; i < options.Length; i++)
        {
            var option = options[i];
            AddOption(option, i);
        }
    }

    public void AddOption(string text, int id)
    {
        var btn = Instantiate(m_buttonOptionTemplate, m_buttonOptionContent);
        btn.transform.GetChild(0).GetComponent<TMP_Text>().text
            = text;
        btn.gameObject.SetActive(true);
        btn.onClick.AddListener(() => { OnAnswered(id); });
    }

    private void OnAnswered(int id)
    {
        OnResult?.Invoke(id);
        Debug.Log("clicked:" + id);
        if (id == 0)
        {
            SceneManager.LoadScene("Mine", LoadSceneMode.Single);
        }
    }
}