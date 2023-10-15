using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingLineServer.Inventory
{
    public class NetworkStructureController 
        : INetworkController
    {
        public static List<Structure> Structures = new List<Structure>();
        private void OnRequestStructures(ReqStructures request, NetPeer peer)
        {
            var packet = new ResStructures();
            packet.Structures = Structures.ToArray();
            PackageSender.SendPacket(peer, packet);
        }

        public void Subscribe(NetPacketProcessor processor)
        {
           
            processor.SubscribeReusable<ReqStructures, NetPeer>(OnRequestStructures);
        }

        public void OnPeerDisconnected(NetPeer peer)
        {
        }

        public void OnPeerConnectionRequest(NetPeer peer, string username)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
        }

        public void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
        {
        }

        public void OnExit()
        {

        }

        public void OnStart()
        {
            Structures.Add(new Structure()
            {
                Id = 0,
                x = 0,
                y = 0,
            });
        }
    }
}
