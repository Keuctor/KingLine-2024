using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayer
{
    public PlayerGear Gear;
    public bool IsLocalPlayer;
    public TMP_Text NameText;
    public Player Player;
    public Transform Transform;
}

public class PlayerController : MonoBehaviour
{
    public static GamePlayer m_localPlayer;

    [SerializeField]
    private Camera m_mainCamera;

    [SerializeField]
    private StructureController m_structureController;

    private bool m_createdPlayers;

    private bool m_isLocalPlayerMoving;

    [SerializeField]
    private PlayerNetworkController m_playerNetworkController;

    private StructureBehaviour m_targetStructure;

    public Dictionary<int, GamePlayer> playerInstances = new();


    private void Start()
    {
        m_playerNetworkController.OnPlayerJoin.AddListener(OnPlayerJoin);
        m_playerNetworkController.OnPlayerLeave.AddListener(OnPlayerLeave);
        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnectedFromServer;
        if (PlayerNetworkController.Players.Count > 0)
        {
            CreatePlayers();
            return;
        }

        m_playerNetworkController.OnPlayerListRefresh.AddListener(CreatePlayers);

        MenuController.Instance.OnOpenMenu.AddListener(OnAnyMenuOpen);
    }

    private void OnAnyMenuOpen()
    {
        m_targetStructure = null;
        ClientSendTargetPosition(new Vector2(m_localPlayer.Player.X, m_localPlayer.Player.Y));
    }

    private void Update()
    {
        if (!m_createdPlayers)
            return;

        foreach (var p in playerInstances)
        {
            var gamePlayer = p.Value;
            var player = p.Value.Player;
            gamePlayer.Transform.position = new Vector2(player.X, player.Y);

            if (Mathf.Abs(player.X - player.TargetX) > float.Epsilon ||
                Mathf.Abs(player.Y - player.TargetY) > float.Epsilon)
            {
                gamePlayer.Gear.SetPlay(true);
                var angle = Vector2.SignedAngle(Vector2.up,
                    new Vector3(player.TargetX, player.TargetY) - gamePlayer.Transform.position);

                var dirr = new Vector2(player.TargetX - player.X, player.TargetY - player.Y).normalized;
                if (dirr.x > 0)
                    gamePlayer.Gear.SetDirection(MoveDirection.Right);
                else
                    gamePlayer.Gear.SetDirection(MoveDirection.Left);
            }
            else
            {
                if (gamePlayer.IsLocalPlayer)
                    if (m_isLocalPlayerMoving)
                    {
                        if (m_targetStructure != null)
                            m_structureController
                                .ShowStructureUI(m_targetStructure.Id);
                        m_isLocalPlayerMoving = false;
                    }

                gamePlayer.Gear.SetPlay(false);
            }
        }


        if (HasPlayerInput())
        {
            Vector2 mousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            m_targetStructure = null;
            foreach (var hit in hits)
                if (hit.transform.CompareTag("Structure"))
                {
                    var structureBehaviour = hit.collider.GetComponent<StructureBehaviour>();
                    var popup = PopupManager.Instance.CreateNew()
                        .CreateImage(structureBehaviour.Icon)
                        .CreateText(structureBehaviour.Name)
                        .CreateText(structureBehaviour.Description)
                        .CreateButton("Go")
                        .CreateButton("Info")
                        .CreateButton("Exit");
                        
                    popup.OnClick.AddListener((x) =>
                    {
                        m_targetStructure = null;
                        if (x == 0)
                        {
                            ClientSendTargetPosition(structureBehaviour.transform.position);
                            m_targetStructure = structureBehaviour;
                        }
                        popup.Destroy();
                    });
                    break;
                }

            if (hits.Length == 0)
            {
                ClientSendTargetPosition(mousePosition);
            }
        }
    }

    public bool HasPlayerInput()
    {

        if (!Application.isMobilePlatform)
        {
            return Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
        }


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
                && eventSystem.currentSelectedGameObject != null);
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }


    private void ClientSendTargetPosition(Vector2 mousePosition)
    {
        m_isLocalPlayerMoving = true;
        m_localPlayer.Player.TargetX = mousePosition.x;
        m_localPlayer.Player.TargetY = mousePosition.y;
        var moveUpdate = new ReqPlayerMove
        {
            x = m_localPlayer.Player.TargetX,
            y = m_localPlayer.Player.TargetY
        };
        NetworkManager.Instance.Send(moveUpdate);
    }

    private void OnDisconnectedFromServer()
    {
        m_createdPlayers = false;

        foreach (var v in playerInstances)
            Destroy(v.Value.Transform.gameObject);

        playerInstances.Clear();
    }

    private void CreatePlayers()
    {
        foreach (var v in PlayerNetworkController.Players) CreatePlayer(v.Value);

        m_createdPlayers = true;
    }

    private void OnPlayerLeave(int obj)
    {
        Destroy(playerInstances[obj].Transform.gameObject);
        playerInstances.Remove(obj);
    }

    private void OnPlayerJoin(int obj)
    {
        CreatePlayer(m_playerNetworkController.GetPlayer(obj));
    }

    private void CreatePlayer(Player pl)
    {
        var p = Instantiate(m_playerNetworkController.HumanPrefab);

        var player = new GamePlayer
        {
            Player = pl
        };
        player.IsLocalPlayer = NetworkManager.LocalPlayerPeerId == player.Player.Id;
        player.Gear = p.GetComponent<PlayerGear>();
        player.Gear.PeerId = player.Player.Id;
        player.Transform = p.transform;
        player.NameText = player.Transform.GetChild(0).GetComponent<TMP_Text>();

        player.NameText.text = player.Player.Name;
        player.Transform.position =
            new Vector2(player.Player.X, player.Player.Y);

        playerInstances.Add(player.Player.Id, player);
        if (player.IsLocalPlayer)
        {
            m_localPlayer = player;
            var camera = FindObjectOfType<CinemachineVirtualCamera>();
            camera.Follow = m_localPlayer.Transform;
            camera.LookAt = m_localPlayer.Transform;

            if (InventoryNetworkController.LocalInventory != null)
            {
                player.Gear.DisplayGear(player.Player.Id);
            }
        }
        else
        {
            // if (InventoryNetworkController.RemoteInventories.ContainsKey(player.Player.Id))
            // {
            //     player.Gear.DisplayGear(player.Player.Id);
            // }
        }
    }
}