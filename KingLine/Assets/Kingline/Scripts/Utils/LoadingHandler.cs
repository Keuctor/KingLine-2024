using UnityEngine;

public class LoadingHandler : MonoBehaviour
{
    private static LoadingHandler m_instance;

    [SerializeField]
    private LoadingHandlerUI m_loadingHandlerUI;

    private LoadingHandlerUI m_loadingHandlerInstance;

    public static LoadingHandler Instance
    {
        get
        {
            m_instance = FindObjectOfType<LoadingHandler>();
            return m_instance;
        }
    }

    public void HideAfterSeconds(float seconds = 0)
    {
        Destroy(m_loadingHandlerInstance.gameObject, seconds);
        m_loadingHandlerInstance = null;
    }

    public void ShowLoading(string text)
    {
        if (m_loadingHandlerInstance != null)
        {
            m_loadingHandlerInstance.AddText(text);
            return;
        }

        m_loadingHandlerInstance = Instantiate(m_loadingHandlerUI);
        m_loadingHandlerInstance.AddText(text);
    }
}