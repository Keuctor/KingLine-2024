using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StructureController : MonoBehaviour
{
    [SerializeField]
    private StructureBehaviour m_structureBehaviour;

    [SerializeField]
    private StructureListSO m_structureList;

    private readonly List<StructureBehaviour> m_structureInstances = new();

    private StructureNetworkController m_structureNetworkController;


    private void Start()
    {
        m_structureNetworkController = NetworkManager.Instance.GetController<StructureNetworkController>();
        if (m_structureNetworkController.Structures.Length > 0 || SceneManager.GetActiveScene().name == "World")
            CreateStructures();
        m_structureNetworkController.OnStructureResponse.AddListener(CreateStructures);
        NetworkManager.Instance.OnDisconnectedFromServer += OnDisconnected;
    }

    private void OnDestroy()
    {
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

        var popup  = PopupManager.Instance.ShowStructureInfo(structureInfo);
        popup.OnClick.AddListener((i) =>
        {
            switch (i)
            {
                case 0:
                {
                    SceneManager.LoadScene("Mine");
                    break;
                }
                case 2:
                {
                    var newPopup = PopupManager.Instance.CreateNew();
                    newPopup.CreateText("What do you want to do here?");
                    newPopup.CreateButton("Sell Items");
                    newPopup.CreateButton("Buy Items");
                    newPopup.OnClick.AddListener((nI) =>
                    {
                        if (nI == 0)
                        {
                            var showItemSelectPopup = PopupManager.Instance.ShowItemSelectPopup();
                            showItemSelectPopup.SelectMode = false;
                        }
                        else
                        {
                            
                        }
                        newPopup.Destroy();
                    });
                    break;
                }
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