using System.Collections.Generic;
using Cinemachine;
using Kingline.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    private PrefabsSO m_prefabs;
    
    private bool m_isLocalPlayerMoving = false;

    private bool m_createdPlayers = false;

    [SerializeField]
    private GameObject m_playerPrefab;

    public static Player m_localPlayer;

    [SerializeField]
    private float m_moveTreshold = 0.16f;

    private StructureInfoUI m_structureInfoUI;

    private StructureBehaviour m_targetStructure;

    [SerializeField]
    private Camera m_mainCamera;

    public Dictionary<int, GameObject> playerInstances = new Dictionary<int, GameObject>();

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
        m_localPlayer.targetX = mousePosition.x;
        m_localPlayer.targetY = mousePosition.y;
        var moveUpdate = new ReqPlayerMove
        {
            x = m_localPlayer.targetX,
            y = m_localPlayer.targetY
        };
        NetworkManager.Instance.Send(moveUpdate);
    }

    private void Start()
    {
        PlayerNetworkController.Instance.OnPlayerJoin.AddListener(OnPlayerJoin);
        PlayerNetworkController.Instance.OnPlayerLeave.AddListener(OnPlayerLeave);
        if (PlayerNetworkController.Instance.Completed)
        {
            CreatePlayers();
            return;
        }
        PlayerNetworkController.Instance.OnPlayerListRefresh.AddListener(CreatePlayers);
    }

    private void CreatePlayers()
    {
        foreach (var v in PlayerNetworkController.Instance.Players)
        {
            CreatePlayer(v.Value);
        }
        m_createdPlayers = true;
    }

    private void OnPlayerLeave(int obj)
    {
        Destroy(playerInstances[obj]);
        playerInstances.Remove(obj);
    }

    private void OnPlayerJoin(int obj)
    {
        CreatePlayer(PlayerNetworkController.Instance.GetPlayer(obj));
    }

    private void Update()
    {
        if (!m_createdPlayers)
            return;

        foreach (var p in PlayerNetworkController.Instance.Players)
        {
            var player = p.Value;
            if (Vector2.Distance(player.Transform.position, new Vector2(player.targetX, player.targetY)) >
                float.Epsilon)
            {
                player.Animator.SetPlay(true);
                var angle = Vector2.SignedAngle(Vector2.up,
                    new Vector3(player.targetX, player.targetY) - player.Transform.position);

                if (angle >= -45 && angle < 45)
                    player.Animator.SetDirection(SpriteAnimator.MoveDirection.UP);
                else if (angle >= 45 && angle < 135)
                    player.Animator.SetDirection(SpriteAnimator.MoveDirection.LEFT);
                else if (angle >= -135 && angle < -45)
                    player.Animator.SetDirection(SpriteAnimator.MoveDirection.RIGHT);
                else
                    player.Animator.SetDirection(SpriteAnimator.MoveDirection.DOWN);

                player.Animator.Animate();
            }
            else
            {
                if (player.IsLocalPlayer)
                {
                    if (m_isLocalPlayerMoving)
                    {
                        if (m_targetStructure != null)
                        {
                            StructureNetworkController.Instance
                                .ShowStructureUI(m_targetStructure.Id);
                        }

                        m_isLocalPlayerMoving = false;
                    }
                }

                player.Animator.SetPlay(false);
            }

            player.Transform.position = Vector2.MoveTowards(player.Transform.position,
                new Vector2(player.targetX, player.targetY),
                player.speed * Time.deltaTime);

            if (Mathf.Abs(player.Transform.position.x - player.x) > 6f ||
                Mathf.Abs(player.Transform.position.y - player.y) > 6f)
            {
                player.targetX = player.x;
                player.targetY = player.y;
                player.Transform.position = new Vector2(player.x, player.y);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            m_targetStructure = null;
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.CompareTag("Structure"))
                {
                    var structureBehaviour = hits[i].collider.GetComponent<StructureBehaviour>();
                    if (m_structureInfoUI == null)
                    {
                        m_structureInfoUI = Instantiate(m_prefabs.StructureInfoUI);
                        m_structureInfoUI.OnClicked.AddListener(i =>
                        {
                            if (i == 0) ClientSendTargetPosition(structureBehaviour.transform.position);
                            m_targetStructure = structureBehaviour;
                            Destroy(m_structureInfoUI.gameObject);
                            m_structureInfoUI = null;
                        });
                    }

                    m_structureInfoUI.SetView(structureBehaviour);
                }
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

    private void CreatePlayer(Player player)
    {
        var p = Instantiate(m_playerPrefab);
        player.Animator = p.GetComponent<SpriteAnimator>();
        player.Transform = p.transform;
        player.NameLabel = player.Transform.GetChild(0).GetComponent<TMP_Text>();

        player.NameLabel.text = player.Name;
        player.Transform.position =
            new Vector2(player.x, player.y);

        playerInstances.Add(player.Id, p);
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
        return Mathf.Abs(m_localPlayer.x - position.x) >= m_moveTreshold
               || Mathf.Abs(m_localPlayer.y - position.y) >= m_moveTreshold;
    }
}