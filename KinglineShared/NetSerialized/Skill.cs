using LiteNetLib.Utils;
using System;


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
