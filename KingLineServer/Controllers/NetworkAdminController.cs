using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


public class NetworkAdminController : INetworkController
{
    public bool RequirePassword = false;

    public string Password = "123";

    public HashSet<string> Admins = new HashSet<string>();

    public void OnExit()
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
    }

    public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {


    }

    public void OnPeerDisconnected(NetPeer peer)
    {
    }

    public void OnStart()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    private void OnRequestPermission(ReqAdminPrivileges request, NetPeer peer)
    {
        if (string.IsNullOrEmpty(request.Password))
        {
            return;
        }
        if (request.Password.Equals(Password))
        {
            var token = KingLine.GetPlayerToken(peer.Id);
            if (Admins.Contains(token))
            {
                SendLog(peer, "STR_YOU_ALREADY_HAVE_PERMISSIONS");
                return;
            }
            Admins.Add(token);
            SendLog(peer, "STR_SUCCED_AT_ADDING_ADMINS");
        }
        else
        {
            SendLog(peer, "STR_ADMIN_REQUEST_PASSWORD");
        }
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ReqAdminPrivileges, NetPeer>(OnRequestPermission);
        processor.SubscribeReusable<ReqRemoteCommand, NetPeer>(OnRemoteCommand);

    }

    private void OnRemoteCommand(ReqRemoteCommand command, NetPeer peer)
    {
        var token = KingLine.GetPlayerToken(peer.Id);
        
        if (!Admins.Contains(token))
        {
            SendLog(peer, "STR_ADMIN_PERMISSION_NEEDED");
            return;
        }

        switch (command.Command)
        {
            case "admin.give":
                try
                {
                    var targetInv = NetworkInventoryController.GetInventory(peer);
                    int itemId = int.Parse(command.Arguments[0]);
                    short count = short.Parse(command.Arguments[1]);

                    if (NetworkInventoryController.InventoryAdd(peer, itemId, count))
                    {
                        SendLog(peer, string.Format("STR_{0}:{1} ADDED_TOYOUR_INVENTORY",itemId,count));
                    }
                    else
                    {
                        SendLog(peer, "STR_YOUR_INVENTORY_FULL");
                    }
                }
                catch (Exception ex)
                {
                    SendLog(peer, ex.ToString());
                }
                break;
        }
    }



    public void SendLog(NetPeer peer, string log)
    {
        PackageSender.SendPacket(peer, new ResConsoleLog()
        {
            Log = log
        }); ;
    }
}
