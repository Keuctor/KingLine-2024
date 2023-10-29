using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayer
{
    public PlayerGear Animator;
    public bool IsLocalPlayer;
    public TMP_Text NameText;
    public Player Player;
    public Transform Transform;
}

public class PlayerController : MonoBehaviour
{
    public static GamePlayer m_localPlayer;

    [SerializeField]
    private PrefabsSO m_prefabs;

    [SerializeField]
    private GameObject m_playerPrefab;

    [SerializeField]
    private Camera m_mainCamera;

    [SerializeField]
    private StructureController m_structureController;

    private bool m_createdPlayers;

    private bool m_isLocalPlayerMoving;

    [SerializeField]
    private PlayerNetworkController m_playerNetworkController;

    private StructureInfoUI m_structureInfoUI;

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
        ClientSendTargetPosition(new Vector2(m_localPlayer.Player.x, m_localPlayer.Player.y));
    }

    private void Update()
    {
        if (!m_createdPlayers)
            return;

        foreach (var p in playerInstances)
        {
            var gamePlayer = p.Value;
            var player = p.Value.Player;
            gamePlayer.Transform.position = new Vector2(player.x, player.y);
          
            if (Mathf.Abs(player.x - player.targetX) > float.Epsilon ||
                Mathf.Abs(player.y - player.targetY) > float.Epsilon)
            {
                gamePlayer.Animator.SetPlay(true);
                var angle = Vector2.SignedAngle(Vector2.up,
                    new Vector3(player.targetX, player.targetY) - gamePlayer.Transform.position);

                var dirr = new Vector2(player.targetX - player.x, player.targetY - player.y).normalized;
                if (dirr.x > 0)
                    gamePlayer.Animator.SetDirection(MoveDirection.Right);
                else
                    gamePlayer.Animator.SetDirection(MoveDirection.Left);
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

                gamePlayer.Animator.SetPlay(false);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            m_targetStructure = null;
            foreach (var hit in hits)
                if (hit.transform.CompareTag("Structure"))
                {
                    var structureBehaviour = hit.collider.GetComponent<StructureBehaviour>();
                    if (m_structureInfoUI == null)
                    {
                        m_structureInfoUI = Instantiate(m_prefabs.StructureInfoUI);
                        m_structureInfoUI.OnClicked.AddListener(x =>
                        {
                            if (x == 0) ClientSendTargetPosition(structureBehaviour.transform.position);
                            m_targetStructure = structureBehaviour;
                            Destroy(m_structureInfoUI.gameObject);
                            m_structureInfoUI = null;
                        });
                    }

                    m_structureInfoUI.SetView(structureBehaviour);
                    break;
                }

            if (hits.Length == 0)
            {
                if (m_structureInfoUI != null)
                {
                    Destroy(m_structureInfoUI.gameObject);
                    m_structureInfoUI = null;
                }
                ClientSendTargetPosition(mousePosition);
            }
        }
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }


    private void ClientSendTargetPosition(Vector2 mousePosition)
    {
        m_isLocalPlayerMoving = true;
        m_localPlayer.Player.targetX = mousePosition.x;
        m_localPlayer.Player.targetY = mousePosition.y;
        var moveUpdate = new ReqPlayerMove
        {
            x = m_localPlayer.Player.targetX,
            y = m_localPlayer.Player.targetY
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
        var p = Instantiate(m_playerPrefab);

        var player = new GamePlayer
        {
            Player = pl
        };
        player.IsLocalPlayer = NetworkManager.LocalPlayerPeerId == player.Player.Id;
        player.Animator = p.GetComponent<PlayerGear>();
        player.Animator.PeerId = player.Player.Id;
        player.Transform = p.transform;
        player.NameText = player.Transform.GetChild(0).GetComponent<TMP_Text>();
        
        player.NameText.text = player.Player.Name;
        player.Transform.position =
            new Vector2(player.Player.x, player.Player.y);

        playerInstances.Add(player.Player.Id, player);
        if (player.IsLocalPlayer)
        {
            m_localPlayer = player;
            var camera = FindObjectOfType<CinemachineVirtualCamera>();
            camera.Follow = m_localPlayer.Transform;
            camera.LookAt = m_localPlayer.Transform;

            if (InventoryNetworkController.LocalInventory != null)
            {
                player.Animator.DisplayGear(player.Player.Id);
            }
        }
        else
        {
            if (InventoryNetworkController.RemoteInventories.ContainsKey(player.Player.Id))
            {
                player.Animator.DisplayGear(player.Player.Id);
            }
        }
    }
}