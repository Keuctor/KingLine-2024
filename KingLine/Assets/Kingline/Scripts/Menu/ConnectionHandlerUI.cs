using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandlerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_userNameInputField;

    [SerializeField]
    private Button m_connectButton;

    [SerializeField]
    private TMP_Text m_addressText;

    [SerializeField]
    private TMP_Text m_versionText;

    [SerializeField]
    private ConnectionDataSO m_connectionData;

    public Action<string> OnConnectClicked;

    private void Awake()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_addressText.text = $"Address: {m_connectionData.Adress}:{m_connectionData.Port}";
        m_versionText.text = $"Version: {m_connectionData.Version}";
        m_userNameInputField.text = PlayerPrefs.GetString("username", "");
        m_connectButton.onClick.RemoveAllListeners();
        m_connectButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(m_userNameInputField.text))
            {
                LoadingHandler.Instance.ShowLoading("USERNAME_LENGTH_ERROR");
                LoadingHandler.Instance.HideAfterSeconds(1);
                return;
            }

            PlayerPrefs.SetString("username", m_userNameInputField.text);
            OnConnectClicked?.Invoke(m_userNameInputField.text);
        });
    }
}