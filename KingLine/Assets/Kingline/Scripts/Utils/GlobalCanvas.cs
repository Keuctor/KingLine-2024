using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCanvas : Singleton<GlobalCanvas>
{
    [SerializeField] private TMP_Text m_latencyText;

    [SerializeField] private TMP_Text M_IdText;

    [SerializeField] private TMP_Text m_errorTemplate;

    [SerializeField] private Transform m_errorContainer;
    private DateTime _lastLogTime;

    private void OnEnable()
    {
        _lastLogTime = DateTime.Now;
        Application.logMessageReceived += OnLogMessageReceived;
    }

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
    {
        if ((DateTime.Now - _lastLogTime).Seconds < 5)
        {
            return;
        }
        
        _lastLogTime = DateTime.Now;
        var errorText = Instantiate(m_errorTemplate, m_errorContainer);
        errorText.text += $"\n"+condition;
        errorText.text += $"\n<size=30>{stacktrace.Substring(0, 220)}</size>";
        errorText.gameObject.SetActive(true);
        errorText.GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(errorText.gameObject);
        });
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    void Start()
    {
    }


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