using LiteNetLib;
using LiteNetLib.Utils;


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
        var token = KingLine.GetPlayerToken(peer.Id);

        var playerLevel = NetworkPlayerLevelController.GetPlayerLevel(token);

        var progression = Progressions[token];
        var lvl = playerLevel;
        foreach (var p in progression)
            lvl -= (p.Value - 1);

        if (lvl > 0)
        {
            foreach (var s in Progressions[token])
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
        var token = KingLine.GetPlayerToken(peer.Id);

        ResPlayerProgression response = new ResPlayerProgression();
        if (Progressions.TryGetValue(token, out var progression))
        {
            response.Skills = progression;
        }
        else
        {
            response.Skills = new Skill[6]
            {
                CreateSkill("Strength",1),
                CreateSkill("Defence",1),
                CreateSkill("Agility",1),
                CreateSkill("Intelligence",1),
                CreateSkill("Charisma",1),
                CreateSkill("Leadership",1),
            };
            Progressions.Add(token, response.Skills);
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

    public void OnUpdate(float deltaTime)
    {
    }
}
