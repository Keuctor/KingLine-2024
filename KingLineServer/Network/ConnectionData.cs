using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ConnectionData
{
    private string m_localAdress = "localhost";

    private string m_serverAdress = "34.150.227.31";

    private int m_port = 9050;

    private bool m_isLocal = true;

    public string Adress =>
        m_isLocal ? m_localAdress : m_serverAdress;

    public int Port =>
        m_port;
    public string Version => "v0.1";
}
