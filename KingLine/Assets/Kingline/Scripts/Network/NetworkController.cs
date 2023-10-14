using UnityEngine;

public abstract class NetworkController : MonoBehaviour
{
    public void Start()
    {
        SubscribeResponse();
        NetworkManager.Instance.OnConnectedToServer += OnConnectedToServer;
        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnectedFromServer;
        if (NetworkManager.Instance.Connected)
        {
            HandleRequest();
        }
    }

    public void OnDestroy()
    {
        UnSubscribeResponse();
        NetworkManager.Instance.OnConnectedToServer -= OnConnectedToServer;
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }

    public virtual void OnConnectedToServer()
    {
        HandleRequest();
    }
    
    public abstract void SubscribeResponse();
    public abstract void HandleRequest();
    public abstract void UnSubscribeResponse();
    public abstract void OnDisconnectedFromServer();
}