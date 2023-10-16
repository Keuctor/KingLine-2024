using KingLineServer.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Reflection.PortableExecutable;

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


public class ReqSkillIncrement
{
    public string SkillName { get; set; }
}
public class ResSkillValueChange
{
    public string SkillName { get; set; }
    public byte Value { get; set; }
}

namespace KingLineServer.Inventory
{
    public class NetworkPlayerProgressionController : INetworkController
    {

        public static Dictionary<string, Skill[]> Progressions
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
            processor.SubscribeReusable<ReqSkillIncrement, NetPeer>(OnSkillIncrement);
        }

        private void OnSkillIncrement(ReqSkillIncrement request, NetPeer peer)
        {
            var player = NetworkPlayerController.Players[peer];
            var playerLevel = NetworkPlayerLevelController.GetPlayerLevel(player.UniqueIdendifier);

            var progression = Progressions[player.UniqueIdendifier];
            var lvl = playerLevel;
            foreach (var p in progression)
                lvl -= (p.Value - 1);

            if (lvl > 0)
            {
                foreach (var s in Progressions[player.UniqueIdendifier])
                {
                    if (s.Name.Equals(request.SkillName))
                    {
                        s.Value++;
                        var response = new ResSkillValueChange()
                        {
                            SkillName = s.Name,
                            Value = s.Value
                        };
                        PackageSender.SendPacket(peer, response);
                    }
                }
            }
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
