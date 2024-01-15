using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


/*
 * - Silahların ve zırhların ek özellikleri + basmak
 * - Pazar yeri insanlar buldukları eşyaları satabilmeli
 * - Açlık
 * - Chat
 * - Trade olması lazım
 * - Item özelliklerinin görünmesi lazım.
 *
 */

public class MineGame : MonoBehaviour
{
    public enum MineType
    {
        STONE,
        BONE
    }

    private static int m_selectedToolIndex = -1;


    [SerializeField]
    private GameObject m_nodePrefab;

    [SerializeField]
    private Vector2 m_minSpawnPosition;

    [SerializeField]
    private Vector2 m_maxSpawnPosition;

    [SerializeField]
    private int m_maxMineCount = 6;

    [SerializeField]
    private MineType m_mineType;

    [SerializeField]
    private bool m_loop;

    [SerializeField]
    private TMP_Text m_selectedToolPropertiesText;

    [SerializeField]
    private Image m_selectedToolImage;

    [SerializeField]
    private TMP_Text m_selectedToolNameText;

    public float ToolModifier;

    [SerializeField]
    private InventoryNetworkController m_inventoryNetworkController;

    [SerializeField]
    private ProgressionNetworkController m_progressionNetworkController;

    private readonly int m_currentCount = 0;

    [FormerlySerializedAs("m_spriteLoader")]
    [SerializeField]
    private MaterialSpriteDatabase m_materialDatabase;

    [SerializeField]
    private AudioManager m_audioManager;

    private void Start()
    {
        for (var i = 0; i < m_maxMineCount; i++) Spawn(true);
        DisplayTool(m_selectedToolIndex);
    }


    public void SelectTool()
    {
        var popup = PopupManager.Instance.CreateNew("ItemSelectPopup");
        var invView = popup.Add(PopupManager.Instance.InventoryView);
        invView.Show(InventoryNetworkController.LocalInventory);
        invView.OnItemSelect.AddListener((i =>
        {
            popup.Destroy();
            DisplayTool(i);
        }));
    }

    private void DisplayTool(int itemId)
    {
        m_selectedToolPropertiesText.text = "Modifier: x" + 0.5f;
        ToolModifier = 0.5f;
        m_selectedToolNameText.text = "Hand";
        m_selectedToolImage.enabled = false;

        if (itemId == -1)
            return;

        var material = ItemRegistry.GetItem(itemId);
        if (material.Type != IType.TOOL) return;

        var toolItemMaterial = (ToolItemMaterial)material;
        m_selectedToolIndex = itemId;
        m_selectedToolPropertiesText.text = "Modifier: x" + toolItemMaterial.ToolValue;
        m_selectedToolImage.sprite = m_materialDatabase.LoadSprite(toolItemMaterial.Id);
        m_selectedToolImage.enabled = true;
        m_selectedToolNameText.text = toolItemMaterial.Name;
        ToolModifier = toolItemMaterial.ToolValue;
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

    private void OnDamage(NodeBehaviour node)
    {
        if (node.IsDead) return;
        var skill = m_progressionNetworkController.GetSkill("Strength");

        node.Damage(10 * (Mathf.Max(1, skill / 2f) * ToolModifier));
        m_audioManager.PlayOnce(SoundType.BREAKING_1, true, 0.3f);
    }


    private void OnNodeCompletePart()
    {
        m_audioManager.PlayOnce(SoundType.BREAKING_2, true, 0.5f);
    }

    private void OnNodeDestroyed()
    {
        if (m_loop)
            StartCoroutine(SpawnAfterSeconds(Random.Range(2, 4f)));

        if (m_mineType == MineType.STONE)
            NetworkManager.Instance.Send(new ReqMineStone());
        else if (m_mineType == MineType.BONE) NetworkManager.Instance.Send(new ReqMineBone());

        m_audioManager.PlayOnce(SoundType.BREAKING_2, true, 0.5f);
    }

    public IEnumerator SpawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Spawn(false);
    }
}