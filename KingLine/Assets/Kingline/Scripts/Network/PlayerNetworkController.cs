using System;
using System.Collections.Generic;
using Cinemachine;
using Kingline.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNetworkController : NetworkController
{
    [SerializeField]
    private GameObject m_playerPrefab;

    [SerializeField]
    private PrefabsSO m_prefabs;

    public float MoveTreshold = 0.16f;

    [SerializeField]
    private Camera m_mainCamera;

    private readonly Dictionary<int, Player> Players = new();

    private bool m_isCreatedPlayers;

    private Player m_localPlayer;

    private StructureInfoUI m_structureInfoUI;

    private StructureBehaviour m_targetStructure;

    private bool m_islocalPlayerMoving = false;

    public override void SubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayers>(OnPlayersResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerPosition>(OnUpdatePlayerPositionResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerMove>(OnPlayerTargetChangeResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerJoin>(OnPlayerJoinedResponse);

        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResPlayerLeave>(OnPlayerLeaveResponse);
    }

    public override void HandleRequest()
    {
        NetworkManager.Instance.Send(new ReqPlayers());
    }

    public override void UnSubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayers>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerPosition>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerMove>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerJoin>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResPlayerLeave>();
    }

    private void Update()
    {
        if (!m_isCreatedPlayers) return;

        foreach (var player in Players.Values)
        {
            if (Vector2.Distance(player.Transform.position, new Vector2(player.targetX, player.targetY)) >
                float.Epsilon)
            {
                #region EDITOR

                if (m_debug)
                {
                    if (m_debugSpriteRenderers.TryGetValue(player.Id, out var renderer))
                    {
                        renderer.transform.position = new Vector2(player.x, player.y);
                    }
                    else
                    {
                        var db = Instantiate(m_debugPlayerTemplate);
                        db.transform.position = new Vector2(player.x, player.y);
                        m_debugSpriteRenderers.Add(player.Id, db);
                    }
                }
                else
                {
                    if (m_debugSpriteRenderers.Count > 0)
                    {
                        foreach (var v in m_debugSpriteRenderers) Destroy(v.Value.gameObject);

                        m_debugSpriteRenderers.Clear();
                    }
                }

                #endregion

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
                    if (m_islocalPlayerMoving)
                    {
                        if (m_targetStructure != null)
                        {
                            StructureNetworkController.Instance
                                .ShowStructureUI(m_targetStructure.Id);
                        }

                        m_islocalPlayerMoving = false;
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

        Players.Add(player.Id, player);

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
        return Mathf.Abs(m_localPlayer.x - position.x) >= MoveTreshold
               || Mathf.Abs(m_localPlayer.y - position.y) >= MoveTreshold;
    }


    public override void OnDisconnectedFromServer()
    {
        ClearPlayers();
    }

    private void ClearPlayers()
    {
        foreach (var p in Players)
            Destroy(p.Value.Transform.gameObject);
        Players.Clear();
        m_isCreatedPlayers = false;
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
        m_islocalPlayerMoving = true;
        m_localPlayer.targetX = mousePosition.x;
        m_localPlayer.targetY = mousePosition.y;
        var moveUpdate = new ReqPlayerMove
        {
            x = m_localPlayer.targetX,
            y = m_localPlayer.targetY
        };
        NetworkManager.Instance.Send(moveUpdate);
    }

    #region EDITOR

    [Header("Editor")]
    [SerializeField]
    private SpriteRenderer m_debugPlayerTemplate;

    [SerializeField]
    private bool m_debug;

    private readonly Dictionary<int, SpriteRenderer> m_debugSpriteRenderers = new();

    #endregion

    #region NETWORK_RESPONSE

    private void OnPlayersResponse(ResPlayers res)
    {
        for (var i = 0; i < res.Players.Length; i++) CreatePlayer(res.Players[i]);
        m_isCreatedPlayers = true;
    }

    private void OnPlayerTargetChangeResponse(ResPlayerMove target)
    {
        var p = Players[target.Id];
        if (p.IsLocalPlayer) return;

        p.targetX = target.x;
        p.targetY = target.y;
    }

    private void OnUpdatePlayerPositionResponse(ResPlayerPosition target)
    {
        var p = Players[target.Id];
        p.x = target.x;
        p.y = target.y;
    }

    private void OnPlayerJoinedResponse(ResPlayerJoin resPlayer)
    {
        CreatePlayer(resPlayer.Player);
    }

    private void OnPlayerLeaveResponse(ResPlayerLeave resPlayer)
    {
        for (var i = 0; i < Players.Count; i++)
            if (Players[i].Id == resPlayer.Player.Id)
            {
                Destroy(Players[i].Transform.gameObject);
                Players.Remove(resPlayer.Player.Id);
                break;
            }
    }

    #endregion
}