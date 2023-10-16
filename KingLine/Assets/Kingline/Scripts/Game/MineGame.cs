using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class MineGame : MonoBehaviour
{
    public enum MineType
    {
        STONE,
        BONE
    }

    [SerializeField]
    private GameObject m_nodePrefab;

    [SerializeField]
    private Vector2 m_minSpawnPosition;

    [SerializeField]
    private Vector2 m_maxSpawnPosition;

    [SerializeField]
    private int m_maxMineCount = 6;

    private int m_currentCount = 0;

    [SerializeField]
    private MineType m_mineType;

    [SerializeField]
    private bool m_loop;

    private void Start()
    {
        for (int i = 0; i < m_maxMineCount; i++)
        {
            Spawn(true);
        }
    }

    public void Spawn(bool instant)
    {
        if (m_currentCount > m_maxMineCount) return;

        var nodeInstance = Instantiate(m_nodePrefab, transform);
        nodeInstance.transform.localPosition = new Vector2(Random.Range(m_minSpawnPosition.x, m_maxSpawnPosition.x)
            , Random.Range(m_minSpawnPosition.y, m_maxSpawnPosition.y));

        if (!instant)
        {
            var scale = nodeInstance.transform.localScale;
            nodeInstance.transform.localScale = Vector3.zero;
            nodeInstance.transform.DOScale(scale, 0.5f);
        }

        nodeInstance.gameObject.SetActive(true);
        var node = nodeInstance.GetComponent<NodeBehaviour>();
        node.SetHealth(100);


        node.OnClick.AddListener(() => { OnDamage(node); });
        node.OnDestroy.AddListener(OnNodeDestroyed);
        node.OnComplete.AddListener(OnNodeCompletePart);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("World");
        }
    }

    private static void OnDamage(NodeBehaviour node)
    {
        if (node.IsDead) return;
        var skill = ProgressionNetworkController.Instance.GetSkill("Strength");
        node.Damage(10*skill);
        AudioManager.Instance.PlayOnce(SoundType.BREAKING_1, true, 0.3f);
    }

    private void OnNodeCompletePart()
    {
        AudioManager.Instance.PlayOnce(SoundType.BREAKING_2, true, 0.5f);
    }

    private void OnNodeDestroyed()
    {
        AudioManager.Instance.PlayOnce(SoundType.BREAKING_2, true, 0.5f);
        if (m_loop)
            StartCoroutine(SpawnAfterSeconds(Random.Range(2, 10f)));
        if (m_mineType == MineType.STONE)
        {
            NetworkManager.Instance.Send(new ReqMineStone());
        }
        else if (m_mineType == MineType.BONE)
        {
            NetworkManager.Instance.Send(new ReqMineBone());
        }
    }

    public IEnumerator SpawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Spawn(false);
    }
}