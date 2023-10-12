using TMPro;
using UnityEngine;

public class LoadingHandlerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_loadingText;

    private void Awake()
    {
        m_loadingText.text = "";
    }

    public void AddText(string text)
    {
        m_loadingText.text = m_loadingText.text + "\n" + text;
    }
}