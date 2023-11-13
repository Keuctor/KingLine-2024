using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NetworkAdminController : INetworkController
{

    public string Password = "123";

    public Dictionary<string, NetPeer> Admins = new Dictionary<string, NetPeer>();

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

    private void OnRequestAdminPrivileges(ReqAdminPrivileges request, NetPeer peer)
    {
        if (string.IsNullOrEmpty(request.Password))
        {
            return;
        }

        if (request.Password.Equals(Password))
        {
            var token = KingLine.GetPlayerToken(peer.Id);

            if (Admins.ContainsKey(token))
            {
                return;
            }

            Admins.Add(token, peer);
            PackageSender.SendPacket(peer, new ResAdminPrivileges()
            {
                IsAdmin = true
            });
        }
    }

    public void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ReqAdminPrivileges, NetPeer>(OnRequestAdminPrivileges);
        processor.SubscribeReusable<ReqAdminCommand, NetPeer>(OnRequestAdminCommand);
    }

    private void OnRequestAdminCommand(ReqAdminCommand command, NetPeer peer)
    {
        var token = KingLine.GetPlayerToken(peer.Id);
        if (!Admins.ContainsKey(token))
        {
            SendLog(peer, "You don't have a permission to use this command.");
            return;
        }

        switch (command.CommandType)
        {
            case 0:
                try
                {
                    var targetInv = NetworkInventoryController.Inventories[token];
                    int itemId = int.Parse(command.CommandValue1);
                    short count = short.Parse(command.CommandValue2);

                    if (NetworkInventoryController.InventoryAdd(peer, itemId, count))
                    {
                        SendLog(peer, $"Item {itemId}:{count} added to your inventory");
                    }
                    else
                    {
                        SendLog(peer, "Can't add item. Inventory full or can't find that ID");
                    }
                    return;
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
