using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Popup
{
    public Transform Container;
    public GameObject Parent;

    public Popup(GameObject obj, Transform container)
    {
        this.Parent = obj;
        this.Container = container;
    }
    public void Destroy()
    {
        Object.Destroy(Parent);
    }

    public UnityEvent<int> OnClick = new();
    
    private int buttonCount = 0;

    public Popup CreateButton(string text)
    {
        var button = Object.Instantiate(PopupManager.Instance.PopupButton, Container);
        button.transform.GetChild(1).GetComponent<TMP_Text>().text = text;
        var index = buttonCount++;
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnClick?.Invoke(index);
        });
        return this;
    }

    public Popup CreateImage(Sprite sprite)
    {
        var image = Object.Instantiate(PopupManager.Instance.PopupImage, Container);
        image.GetComponent<Image>().sprite = sprite;
        return this;
    }

    public Popup CreateText(string text)
    {
        var tc = Object.Instantiate(PopupManager.Instance.PopupText, Container);
        tc.GetComponent<TMP_Text>().text = text;
        return this;
    }
}

[CreateAssetMenu]
public class PopupManager : ScriptableObject
{
    private static PopupManager m_instance;

    public static PopupManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Resources.LoadAll<PopupManager>("Manager")[0];
            }

            return m_instance;
        }
    }


    public GameObject Canvas;
    public GameObject PopupContainer;
    public GameObject PopupButton;
    public GameObject PopupText;
    public GameObject PopupImage;

    public PrefabsSO Prefabs;

    public ItemSelectPopup ShowItemSelectPopup()
    {
        return Instantiate(Prefabs.ItemSelectPopup);
    }

    public Popup CreateNew()
    {
        var canvas = Instantiate(Canvas);
        var container = Instantiate(PopupContainer, canvas.transform);

        MenuController.Instance.Menus.Push(canvas);
        Popup popup = new Popup(canvas,container.transform);
        return popup;
    }

    public Popup ShowStructureInfo(StructureInfo info)
    {
        var canvas = Instantiate(Canvas);
        var container = Instantiate(PopupContainer, canvas.transform);

        MenuController.Instance.Menus.Push(canvas);
        Popup popup = new Popup(canvas,container.transform);
        popup.CreateImage(info.Icon)
            .CreateText(info.EnterDescription);

        var options = info.Options;
        for (var i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var x = i;
            popup.CreateButton(option);
        }
        return popup;
    }
}