using LiteNetLib;
using LiteNetLib.Utils;

    public interface INetworkController
    {
        public void OnPeerDisconnected(NetPeer peer);
        public void OnPeerConnectionRequest(NetPeer peer,string idendifier,string username);
        public void OnPeerConnected(NetPeer peer);
        public void Subscribe(NetPacketProcessor processor);
        void OnExit();
        void OnStart();
    }
