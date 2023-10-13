using TMPro;
using UnityEngine;

public class GlobalCanvas : Singleton<GlobalCanvas>
{
    [SerializeField]
    private TMP_Text m_latencyText;

    [SerializeField]
    private TMP_Text M_IdText;

    public void SetLatency(int latency)
    {
        if (latency == -1)
        {
            m_latencyText.text = "";
            return;
        }

        m_latencyText.text = $"Ping: {latency}ms";
    }

    public void SetId(int peerId)
    {
        M_IdText.text = $"Id {peerId}";
    }
}