using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class MenuNavigation
{
    public Button Button;
    public GameObject UI;
}

public class MenuController : Singleton<MenuController>
{
    [SerializeField]
    private List<MenuNavigation> m_menuNavigation = new();

    [SerializeField]
    private GameObject m_blocker;

    private bool m_isAnyOpen => m_menuNavigation.Any(t => t.UI.activeSelf);

    public UnityEvent OnOpenMenu = new();


    private void Start()
    {
        foreach (var m in m_menuNavigation)
        {
            m.Button.onClick.AddListener(() =>
            {
                OpenUI(m);
            });
        }
    }

    public void CloseAll()
    {
        m_blocker.gameObject.SetActive(false);
        foreach (var n in m_menuNavigation)
        {
            n.UI.SetActive(false);
            n.Button.transform.DOScale(Vector2.one, 0.2f);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_isAnyOpen)
                CloseAll();
            else
            {
                if (!SceneManager.GetActiveScene().name.Equals("World"))
                {
                    SceneManager.LoadScene("World");
                }
            }
        }
    }

    public void OpenUI(MenuNavigation navigation)
    {
        if (navigation.UI.activeSelf)
        {
            CloseAll();
            return;
        }
        
        CloseAll();
        navigation.UI.gameObject.SetActive(true);
        navigation.Button.DOKill();
        navigation.Button.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBounce);
        m_blocker.gameObject.SetActive(true);
        OnOpenMenu?.Invoke();
    }
}