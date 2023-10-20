using System.Collections.Generic;
using Cinemachine;
using Kingline.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayer
{
    public SpriteAnimator Animator;
    public bool IsLocalPlayer;
    public TMP_Text NameText;
    public Player Player;
    public Transform Transform;
}

public class PlayerMovementController : MonoBehaviour
{
    public static GamePlayer m_localPlayer;

    [SerializeField]
    private PrefabsSO m_prefabs;

    [SerializeField]
    private GameObject m_playerPrefab;

    [SerializeField]
    private float m_moveTreshold = 0.16f;


    [SerializeField]
    private Camera m_mainCamera;

    [SerializeField]
    private StructureController m_structureController;

    private bool m_createdPlayers;

    private bool m_isLocalPlayerMoving;

    public PlayerNetworkController m_playerNetworkController;

    private StructureInfoUI m_structureInfoUI;

    private StructureBehaviour m_targetStructure;

    public Dictionary<int, GamePlayer> playerInstances = new();

    private void Start()
    {
        m_playerNetworkController = NetworkManager.Instance.GetController<PlayerNetworkController>();
        m_playerNetworkController.OnPlayerJoin.AddListener(OnPlayerJoin);
        m_playerNetworkController.OnPlayerLeave.AddListener(OnPlayerLeave);
        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnectedFromServer;
        if (m_playerNetworkController.Players.Count > 0)
        {
            CreatePlayers();
            return;
        }

        m_playerNetworkController.OnPlayerListRefresh.AddListener(CreatePlayers);
    }

    private void Update()
    {
        if (!m_createdPlayers)
            return;

        foreach (var p in playerInstances)
        {
            var gamePlayer = p.Value;
            var player = p.Value.Player;
            if (Vector2.Distance(gamePlayer.Transform.position, new Vector2(player.targetX, player.targetY)) >
                float.Epsilon)
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

            gamePlayer.Transform.position = Vector2.MoveTowards(gamePlayer.Transform.position,
                new Vector2(player.targetX, player.targetY),
                player.speed * Time.deltaTime);

            if (Mathf.Abs(gamePlayer.Transform.position.x - player.x) > 6f ||
                Mathf.Abs(gamePlayer.Transform.position.y - player.y) > 6f)
            {
                player.targetX = player.x;
                player.targetY = player.y;
                gamePlayer.Transform.position = new Vector2(player.x, player.y);
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

        if (IsLocalPlayerMoved()) ClientSendPositionUpdate();
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnectedFromServer;
    }

    private void ClientSendPositionUpdate()
    {
        var positionUpdate = new ResPlayerPosition
        {
            x = m_localPlayer.Transform.position.x,
            y = m_localPlayer.Transform.position.y
        };
        NetworkManager.Instance.Send(positionUpdate);
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
        foreach (var v in m_playerNetworkController.Players) CreatePlayer(v.Value);

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
        player.Animator = p.GetComponent<SpriteAnimator>();
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
        }
    }

    private bool IsLocalPlayerMoved()
    {
        var position = m_localPlayer.Transform.position;
        return Mathf.Abs(m_localPlayer.Player.x - position.x) >= m_moveTreshold
               || Mathf.Abs(m_localPlayer.Player.y - position.y) >= m_moveTreshold;
    }
}