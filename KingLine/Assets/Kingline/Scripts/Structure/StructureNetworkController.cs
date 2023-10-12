using System.Collections.Generic;
using System.Linq;
using Kingline.Scripts.Structure;
using UnityEngine;

public class StructureNetworkController : NetworkController
{
    [SerializeField]
    private StructureBehaviour m_structureBehaviour;

    [SerializeField]
    private StructureListSO m_structureList;

    private readonly List<StructureBehaviour> m_structureInstances = new();

    [SerializeField]
    private StructureUI m_structureUITemplate;

    private StructureUI m_structureUIInstance;

    private static StructureNetworkController m_instance;

    public static StructureNetworkController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<StructureNetworkController>();
            }
            return m_instance;
        }
    }

    private void Start()
    {
        m_networkManager.NetPacketProcessor
            .SubscribeReusable<ResStructures>(OnStructuresResponse);
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
        m_structureUIInstance.OnResult.AddListener((index) =>
        {
            if (index == structureInfo.Options.Length - 1)
            {
                Destroy(m_structureUIInstance.gameObject);
                m_structureUIInstance = null;
            }
        });
    }

    private void OnStructuresResponse(ResStructures obj)
    {
        foreach (var structure in obj.Structures)
            CreateStructure(structure);

        LoadingHandler.Instance.ShowLoading("Completed...");
        LoadingHandler.Instance.HideAfterSeconds(0.1f);
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

    public override void OnConnectedToServer()
    {
        LoadingHandler.Instance.ShowLoading("Loading structures...");
        m_networkManager.Send(new ReqStructures());
    }

    public override void OnDisconnectedFromServer()
    {
        foreach (var v in m_structureInstances)
            Destroy(v.gameObject);
    }
}