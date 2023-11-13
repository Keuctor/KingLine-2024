using LiteNetLib;
using LiteNetLib.Utils;
using Mono.CSharp;
using QFSW.QC;
using UnityEngine;

[CreateAssetMenu]
public class NetworkAdminController : NetworkController
{
    public override void OnPeerDisconnected(NetPeer peer)
    {
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }

    public override void OnPeerConnected(NetPeer peer)
    {
    }

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResAdminPrivileges>(OnResAdminPrivileges);
        processor.SubscribeReusable<ResConsoleLog>(OnResConsoleLog);
    }

    private void OnResConsoleLog(ResConsoleLog obj)
    {
        QuantumConsole.Instance.LogToConsole(obj.Log);
    }

   
    
    [Command("admin.give")]
    public static void AddItem(MaterialType id, int count)
    {
        NetworkManager.Instance.Send(new ReqAdminCommand()
        {
            CommandType = 0,
            CommandValue1 = ((int)id).ToString(),
            CommandValue2 = count.ToString()
        });
    }

    [Command("admin.request")]
    public static void SendAdminRequest(string message)
    {
        NetworkManager.Instance.Send(new ReqAdminPrivileges()
        {
            Password = message
        });
    }

    private void OnResAdminPrivileges(ResAdminPrivileges response)
    {
        if (response.IsAdmin)
        {
            QuantumConsole.Instance.LogToConsole("You have admin privileges.");
        }
    }

    public override void OnExit()
    {
    }

    public override void OnStart()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
    }
}