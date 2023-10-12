using System;
using UnityEngine;

public abstract class NetworkController : MonoBehaviour
{
    [SerializeField]
    protected NetworkManager m_networkManager;

    private void OnEnable()
    {
        m_networkManager.OnConnectedToServer += OnConnectedToServer;
        m_networkManager.OnDisconnectedFromServer += OnDisconnectedFromServer;
    }

    private void OnDisable()
    {
        m_networkManager.OnConnectedToServer -= OnConnectedToServer;
        m_networkManager.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }

    public abstract void OnDisconnectedFromServer();

    public abstract void OnConnectedToServer();
}