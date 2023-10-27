using LiteNetLib.Utils;

public partial class Structure : INetSerializable
{
    public int Id { get; set; }
    public float x { get; set; }
    public float y { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(x);
        writer.Put(y);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        x = reader.GetFloat();
        y = reader.GetFloat();
    }
}
