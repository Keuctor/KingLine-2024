using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class NetworkController<T> : MonoBehaviour 
where T : NetworkController<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = (T)this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (Instance != this)
            return;

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SubscribeResponse();
        OnStart();
        NetworkManager.Instance.OnConnectedToServer += OnConnectedToServer;
        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnectedFromServer;
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        OnStart();
    }

    public void OnDestroy()
    {
        if (Instance != this)
            return;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        UnSubscribeResponse();
        NetworkManager.Instance.OnConnectedToServer -= OnConnectedToServer;
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }

    public virtual void OnConnectedToServer()
    {
        if (Instance != this)
            return;
        HandleRequest();
    }

    public virtual void OnStart()
    {
    }

    public abstract void SubscribeResponse();
    public abstract void HandleRequest();
    public abstract void UnSubscribeResponse();
    public abstract void OnDisconnectedFromServer();
}