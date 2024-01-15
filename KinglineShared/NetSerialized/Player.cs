using LiteNetLib.Utils;

public partial class Player : INetSerializable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float TargetY { get; set; }
    public float TargetX { get; set; }
    public float Speed { get; set; }
    public int Gold { get; set; }
    public ulong InventoryId { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(X);
        writer.Put(Y);
        writer.Put(TargetY);
        writer.Put(TargetX);
        writer.Put(Speed);
        writer.Put(Gold);
    }
    public void Deserialize(NetDataReader reader)
    {
        this.Id = reader.GetInt();
        this.Name = reader.GetString(16);
        this.X = reader.GetFloat();
        this.Y = reader.GetFloat();
        this.TargetY = reader.GetFloat();
        this.TargetX = reader.GetFloat();
        this.Speed = reader.GetFloat();
        this.Gold = reader.GetInt();
    }
}
