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

    [SerializeField]
    private StructureUI m_structureUITemplate;

    private readonly List<StructureBehaviour> m_structureInstances = new();

    private StructureNetworkController m_structureNetworkController;

    private StructureUI m_structureUIInstance;

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

        if (m_structureUIInstance != null)
        {
            Destroy(m_structureUIInstance.gameObject);
            m_structureUIInstance = null;
        }

        m_structureUIInstance = Instantiate(m_structureUITemplate);
        m_structureUIInstance.OnResult.RemoveAllListeners();
        m_structureUIInstance.SetContext(structureInfo);
        m_structureUIInstance.OnResult.AddListener(index =>
        {
            if (index == structureInfo.Options.Length - 1)
            {
                Destroy(m_structureUIInstance.gameObject);
                m_structureUIInstance = null;
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