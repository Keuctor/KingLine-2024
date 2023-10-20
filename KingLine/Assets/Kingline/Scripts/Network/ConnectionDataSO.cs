using UnityEngine;

[CreateAssetMenu]
public class ConnectionDataSO : ScriptableObject
{
    [SerializeField]
    private string m_localAdress = "localhost";

    [SerializeField]
    private string m_serverAdress = "34.150.227.31";

    [SerializeField]
    private int m_port = 9050;

    [SerializeField]
    private bool m_isLocal = true;

    public bool Debug = true;

    public string Adress =>
        m_isLocal ? m_localAdress : m_serverAdress;

    public int Port =>
        m_port;

    public string Version => "v0.1";
}