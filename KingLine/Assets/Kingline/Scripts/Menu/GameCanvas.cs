using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvas : Singleton<GameCanvas>
{
    private static Vector2 m_lastLookAroundPosition
        = new Vector2(-9999, -9999);

    [SerializeField]
    private Button m_lookAroundButton;

    private void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private bool check;

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        check = arg1.name == "World";
        m_lookAroundButton.gameObject.SetActive(false);
    }

    bool CanCheckAround()
    {
        if (PlayerMovementController.m_localPlayer == null)
            return false;

        if (PlayerMovementController.m_localPlayer.Transform == null)
            return false;

        return Vector2.Distance(m_lastLookAroundPosition,
            PlayerMovementController.m_localPlayer.Transform.position) >= 5;
    }

    private void Update()
    {
        if (check)
        {
            m_lookAroundButton.gameObject.SetActive(CanCheckAround());
        }
    }

    public void LookAround()
    {
        m_lastLookAroundPosition = PlayerMovementController.m_localPlayer.Transform.position;
        SceneManager.LoadScene("Around");
    }
}