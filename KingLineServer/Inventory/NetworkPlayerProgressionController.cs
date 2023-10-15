using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

[Serializable]
public class Skill : INetSerializable
{
    public string Name { get; set; }
    public byte Value { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put(Value);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        Value = reader.GetByte();
    }
}

public class ReqPlayerProgression
{
}

public class ResPlayerProgression
{
    public Skill[] Skills { get; set; }
}

namespace KingLineServer.Inventory
{
    public class NetworkPlayerProgressionController : INetworkController
    {

        public Dictionary<string, Skill[]> Progressions
             = new Dictionary<string, Skill[]>();
        public void OnStart()
        {
            //Load progressions
        }
        public void OnExit()
        {
            //Save progressions
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
        public void Subscribe(NetPacketProcessor processor)
        {
            processor.RegisterNestedType(() =>
            {
                return new Skill();
            });

            processor.SubscribeReusable<ReqPlayerProgression, NetPeer>(OnPlayerProgressionRequest);
        }

        private void OnPlayerProgressionRequest(ReqPlayerProgression request, NetPeer peer)
        {
            var player = NetworkPlayerController.Players[peer];

            ResPlayerProgression response = new ResPlayerProgression();
            if (Progressions.TryGetValue(player.UniqueIdendifier, out var progression))
            {
                response.Skills = progression;
            }
            else
            {
                response.Skills = new Skill[5]
                {
                    CreateSkill("Strength",1),
                    CreateSkill("Agility",1),
                    CreateSkill("Intelligence",1),
                    CreateSkill("Charisma",1),
                    CreateSkill("Leadership",1),
                };
                Progressions.Add(player.UniqueIdendifier, response.Skills);
            }
            PackageSender.SendPacket(peer, response);
        }
        public Skill CreateSkill(string name, byte value)
        {
            return new Skill()
            {
                Name = name,
                Value = value
            };
        }
    }
}
