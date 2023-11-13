using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Popup
{
    public Transform Container;
    public GameObject Parent;
    public string Name;

    public Popup(string name,GameObject obj, Transform container)
    {
        Name = name;
        this.Parent = obj;
        this.Container = container;
        MenuController.Instance.Popups.Add(this);
    }
    public void Destroy()
    {
        MenuController.Instance.Popups.Remove(this);
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

    public T Add<T>(T component) where T : MonoBehaviour
    {
        var instantiate = Object.Instantiate(component, Container);
        return instantiate;
    }

    public Transform Add(GameObject transform)
    {
        return Object.Instantiate(transform,Container).transform;
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


    [Header("Simple")]
    public GameObject Canvas;
    public GameObject PopupContainer;
    public GameObject PopupButton;
    public GameObject PopupText;
    public GameObject PopupImage;

    [Header("Prefabs")]
    public ItemSelectPopup ItemSelectPopup;


    [Header("Component")]
    public PlayerTeamView PlayerTeamView;

    public PlayerSkillView PlayerSkillView;
    
    public PlayerNameView PlayerNameView;
    
    public PlayerLevelView PlayerLevelView;

    public PlayerSkillPointView PlayerSkillPointView;

    public InventoryView InventoryView;
    public SellItemView SellItemView;
    public InventoryView ScrollableInventoryView;

    public ItemInfoView ItemInfoView;

    public CharacterTextureView CharacterTextureView;

    [FormerlySerializedAs("PlayerGearView")]
    public PlayerGearInventoryView PlayerGearInventoryView;
    

    public ItemSelectPopup ShowItemSelectPopup()
    {
        return Instantiate(ItemSelectPopup);
    }

    public Popup CreateNew(string name="Popup(Default)")
    {
        var canvas = Instantiate(Canvas);
        canvas.name = name;
        var container = Instantiate(PopupContainer, canvas.transform);

        Popup popup = new Popup(name,canvas,container.transform);
        return popup;
    }
}