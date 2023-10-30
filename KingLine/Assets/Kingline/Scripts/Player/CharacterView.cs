using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [SerializeField]
    private GameObject m_characterCamera;

    private Character m_character;

    [SerializeField]
    private TMP_Text m_nameLabel;

    [Header("Upgrade")]
    [SerializeField]
    private GameObject m_upgradePanel;

    [SerializeField]
    private Button m_upgradeButton;

    [SerializeField]
    private TMP_Text m_priceLabel;

    [Header("Xp")]
    [SerializeField]
    private GameObject m_xpSliderPanel;

    [SerializeField]
    private Slider m_xpSlider;

    [SerializeField]
    private TMP_Text m_xpSliderText;


    [SerializeField]
    private TMP_Text m_armorText;

    [SerializeField]
    private TMP_Text m_weaponText;

    private GameObject m_charCamera;

    public UnityEvent OnUpgradeClicked = new();

    [SerializeField]
    private CharacterTextureView m_characterTextureView;

    public void SetUpgrade(float upgradePrice)
    {
        this.m_upgradePanel.gameObject.SetActive(true);
        this.m_priceLabel.text = upgradePrice + "";
        this.m_upgradeButton.interactable = PlayerNetworkController.LocalPlayer.Currency >= upgradePrice;
        this.m_upgradeButton.onClick.AddListener(OnUpgradeClick);
    }

    public void SetXp(int xp, int maxXp)
    {
        this.m_xpSliderPanel.gameObject.SetActive(true);

        m_xpSlider.interactable = false;
        this.m_xpSlider.maxValue = maxXp;
        this.m_xpSlider.minValue = 0;
        this.m_xpSlider.value = xp;

        this.m_xpSliderText.text = $"{xp}/{maxXp}";
    }

    private void OnUpgradeClick()
    {
        OnUpgradeClicked?.Invoke();
    }

    public void Show(string characterName, ItemStack[] items)
    {
        m_nameLabel.text = characterName;
        m_characterTextureView.Show(items);
        this.m_armorText.text = m_characterTextureView.Armor + "";
        this.m_weaponText.text = m_characterTextureView.Strength + "";
    }

    private void OnDestroy()
    {
        Destroy(m_charCamera.gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}