using LiteNetLib.Utils;

public class Team : INetSerializable
{
    public int Id { get; set; }
    public TeamMember[] Members { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        this.Id = reader.GetInt();
        ushort count = reader.GetUShort();
        Members = new TeamMember[count];
        for (int i = 0; i < Members.Length; i++)
        {
            Members[i] = new TeamMember();
            Members[i].Deserialize(reader);
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put((ushort)Members.Length);
        foreach (TeamMember member in Members)
        {
            member.Serialize(writer);
        }
    }
}

public class TeamMember : INetSerializable
{
    public int Id { get; set; }
    public short Count { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        this.Id = reader.GetInt();
        this.Count = reader.GetShort();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(this.Id);
        writer.Put(this.Count);
    }
}
