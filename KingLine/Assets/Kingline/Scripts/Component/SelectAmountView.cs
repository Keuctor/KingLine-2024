using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectAmountView : MonoBehaviour
{
    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private Slider m_amountSlider;

    [SerializeField]
    private TMP_Text m_amountText;

    [SerializeField]
    private Button m_doneButton;

    [SerializeField]
    private Button m_exitButton;

    [SerializeField]
    private TMP_InputField m_amountInputField;

    private int value;
    private int max;
    private int min;

    public UnityEvent OnDone = new();

    public short Value => (short)value;


    private void Start()
    {
        m_icon.gameObject.SetActive(false);
        m_amountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        m_amountInputField.onValueChanged.AddListener(OnAmountInputValueChanged);
        m_doneButton.onClick.AddListener(() => { OnDone?.Invoke();});
        m_exitButton.onClick.AddListener(() => { Destroy(gameObject);});
    }

    private void OnAmountInputValueChanged(string inputStr)
    {
        if (int.TryParse(inputStr, out int newValue))
        {
            if (newValue > max || newValue < min)
            {
                this.m_amountInputField.DeactivateInputField();
                this.m_amountInputField.SetTextWithoutNotify(value + "");
            }
            else
            {
                m_amountSlider.value = newValue;
                return;
            }
        }
        this.m_amountInputField.SetTextWithoutNotify(value + "");
    }

    private void OnSliderValueChanged(float newValue)
    {
        value = (int)newValue;
        m_amountSlider.SetValueWithoutNotify(value);
        m_amountInputField.SetTextWithoutNotify(value.ToString());
        m_amountText.text = $"{value}/{max}";
    }

    public void SetIcon(Sprite sprite)
    {
        m_icon.gameObject.SetActive(true);
        m_icon.transform.localScale = Vector3.zero;
        m_icon.sprite = sprite;
        m_icon.transform.DOScale(Vector3.one, 0.7f);
    }

    public void SetValue(int value, int min, int max)
    {
        this.min = min;
        this.value = value;
        this.max = max;
        m_amountSlider.minValue = min;
        m_amountSlider.maxValue = max;
        m_amountSlider.value = this.value;
        OnSliderValueChanged(this.value);
    }
}