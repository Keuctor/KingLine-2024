using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StructureController : MonoBehaviour
{
    
    [Header("Dependency")]
    
    [SerializeField]
    private MaterialSpriteDatabase m_materialSpriteDatabase;
    
    [SerializeField]
    private StructureNetworkController m_structureNetworkController;

    [SerializeField]
    private TeamNetworkController m_teamNetworkController;

    [SerializeField]
    private StructureListSO m_structureList;
    
    [SerializeField]
    private StructureBehaviour m_structureBehaviour;
    
  

    private readonly List<StructureBehaviour> m_structureInstances = new();
    

    private void Start()
    {
        if (m_structureNetworkController.Structures.Length > 0 || SceneManager.GetActiveScene().name == "World")
            CreateStructures();
        m_structureNetworkController.OnStructureResponse.AddListener(CreateStructures);

        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnected;

        m_teamNetworkController.OnVolunteersResponse
            .AddListener(OnVolunteersResponse);
    }

    private void OnVolunteersResponse(int structureId, int troopId, short count)
    {
        var popup = PopupManager.Instance.CreateNew();
        if (count <= 0)
        {
            popup.CreateText("No one wants to join you.");
            popup.CreateText("<size=32>(Tip: Improve your Leadership skill)</size>");
            popup.CreateButton("Leave...");
            popup.OnClick.AddListener((x) => { popup.Destroy(); });
            return;
        }

        var troop = TroopRegistry.GetTroop(troopId);
        popup.CreateText(
            $"<b>{count}</b> <b>{troop.Name}</b> wants to join your team but they want <b>{troop.Price * count}</b> gold for it");
        popup.CreateButton($"Take them all");
        popup.CreateButton($"Leave...");
        popup.OnClick.AddListener((i) =>
        {
            if (i == 0)
            {
                NetworkManager.Instance.Send(new ReqBuyVolunteers()
                {
                    StructureId = structureId,
                    Count = count,
                    Id = troopId
                });
            }

            popup.Destroy();
        });
    }

    private void OnDestroy()
    {
        m_teamNetworkController.OnVolunteersResponse
            .RemoveListener(OnVolunteersResponse);
        NetworkManager.Instance.OnDisconnectedFromServer -= OnDisconnected;
    }

    private void OnDisconnected()
    {
        ClearStructureObjects();
    }

    private void CreateStructures()
    {
        if (m_structureInstances.Count != 0)
        {
            Debug.Log("[STRUCTURE_CREATION_SKIP]");
            return;
        }

        foreach (var structure in m_structureNetworkController.Structures)
            CreateStructure(structure);

        LoadingHandler.Instance.ShowLoading("Completed...");
        LoadingHandler.Instance.HideAfterSeconds(0.1f);
    }

    public void ShowStructureUI(int structureId)
    {
        var structureInfo = m_structureList.GetStructureInfo(structureId);

        var popup = PopupManager.Instance.CreateNew();
        popup.CreateImage(structureInfo.Icon)
            .CreateText(structureInfo.EnterDescription);

        var options = structureInfo.Options;
        for (var i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var x = i;
            popup.CreateButton(option);
        }

        popup.OnClick.AddListener((i) =>
        {
            switch (i)
            {
                case 0:
                {
                    SceneManager.LoadScene("Mine");
                    break;
                }
                case 1:
                {
                    var newPopup = PopupManager.Instance.CreateNew();
                    newPopup.CreateText(
                        "The town is in a poor state; nobody wants to live here. You see some people lying on the ground, hungry.");
                    newPopup.CreateButton("Gather Volunteers");
                    newPopup.CreateButton("Ask for job");
                    newPopup.CreateButton("Leave");
                    newPopup.OnClick.AddListener(ni =>
                    {
                        if (ni == 0)
                        {
                            m_teamNetworkController.RequestVolunteers(structureId);
                        }

                        if (ni == 1)
                        {
                        }

                        newPopup.Destroy();
                    });
                    break;
                }
                case 2:
                {
                    var newPopup = PopupManager.Instance.CreateNew();
                    newPopup.CreateText("What do you want to do here?");
                    newPopup.CreateButton("Sell Items");
                    newPopup.CreateButton("Buy Items");
                    newPopup.CreateButton("Leave");
                    newPopup.OnClick.AddListener((nI) =>
                    {
                        if (nI == 0)
                        {
                            var showItemSelectPopup = PopupManager.Instance.CreateNew("ItemSellPopup");
                            showItemSelectPopup.CreateText("Madaca's City");
                            var sellItemView = showItemSelectPopup.Add(PopupManager.Instance.SellItemView);
                            showItemSelectPopup.CreateText("Heuctor");
                             var inv2 =  showItemSelectPopup.Add(PopupManager.Instance.InventoryView);
                             inv2.ShowLocalPlayerInventory();
                             inv2.ShowInfo = false;
                             
                             inv2.OnItemSelect.AddListener((itemId =>
                             {
                                 var selectedItem = ItemStackView.SelectedIndex;
                                 sellItemView.SetItemId(selectedItem);
                             }));
                        }
                        newPopup.Destroy();
                    });
                    break;
                }
                default:
                    popup.Destroy();
                    break;
            }
        });
    }


    private void CreateStructure(Structure structure)
    {
        var structureSO = m_structureList.Structures
            .FirstOrDefault(t => t.Id == structure.Id);

        var structureBehaviour = Instantiate(m_structureBehaviour);
        structureBehaviour.transform.position = new Vector2(structure.x, structure.y);
        structureBehaviour.Icon = structureSO.Icon;
        structureBehaviour.Name = structureSO.Name;
        structureBehaviour.Description = structureSO.Description;
        structureBehaviour.Id = structureSO.Id;

        m_structureInstances.Add(structureBehaviour);
    }

    private void ClearStructureObjects()
    {
        foreach (var v in m_structureInstances)
            Destroy(v.gameObject);
        m_structureInstances.Clear();
    }
}