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
    [SerializeField]
    private GameObject m_blocker;

    [NonSerialized]
    public readonly UnityEvent OnOpenMenu = new();

    public List<Popup> Popups = new List<Popup>();

    private CharacterTextureView m_characterTextureView;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Popups.Count > 0)
            {
                var p = Popups[^1];
                if (p != null)
                {
                    p.Destroy();
                    Popups.Remove(p);
                    p = null;
                }
            }
            else
            {
                if (!SceneManager.GetActiveScene().name.Equals("World"))
                    SceneManager.LoadScene("World");
            }
        }
    }

    public void OpenPlayerInventory()
    {
        var activePopup = Popups.FirstOrDefault(t => t.Name == "InventoryUI");
        if (activePopup != null)
        {
            activePopup.Destroy();
            Popups.Remove(activePopup);
            return;
        }

        var popup = PopupManager.Instance.CreateNew("InventoryUI");
        var characterTextureView = popup.Add(PopupManager.Instance.CharacterTextureView);
        characterTextureView.ShowLocalPlayerGear();
        popup.Add(PopupManager.Instance.PlayerGearInventoryView);
        var invView = popup.Add(PopupManager.Instance.InventoryView);
        invView.ShowLocalPlayerInventory();
        Popups.Add(popup);
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