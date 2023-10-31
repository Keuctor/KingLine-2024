using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MenuController : Singleton<MenuController>
{
 
    public readonly Stack<GameObject> Menus = new();

    [SerializeField]
    private GameObject m_blocker;
    [NonSerialized]
    public readonly UnityEvent OnOpenMenu = new();

    public List<Popup> Popups = new List<Popup>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var p = Popups[^1];
            if (p != null)
            {
                p.Destroy();
                Popups.Remove(p);
                p= null;
            }
        }
    }

    public void OpenPlayerUI()
    {
        var activePopup = Popups.FirstOrDefault(t => t.Name == "PlayerUI");
        if (activePopup != null)
        {
            activePopup.Destroy();
            Popups.Remove(activePopup);
            return;
        }

        var popup = PopupManager.Instance.CreateNew("PlayerUI");
        popup.Add(PopupManager.Instance.PlayerNameView);
        popup.Add(PopupManager.Instance.PlayerLevelView);
        popup.Add(PopupManager.Instance.PlayerSkillPointView);
        popup.CreateText("Skills");
        popup.Add(PopupManager.Instance.PlayerSkillView);
        popup.CreateText("Team");
        popup.Add(PopupManager.Instance.PlayerTeamView);
        Popups.Add(popup);
    }
}