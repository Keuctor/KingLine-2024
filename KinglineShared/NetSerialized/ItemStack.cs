using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;

public class ItemStack : INetSerializable
{
    public int Id { get; set; }
    public short Count { get; set; }
    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        Count = reader.GetShort();
    }
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Count);
    }
}
