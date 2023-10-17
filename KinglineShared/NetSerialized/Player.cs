using LiteNetLib.Utils;



public partial class Player : INetSerializable
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string Name { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float targetX { get; set; }
    public float targetY { get; set; }
    public float speed { get; set; }
    public int Coin { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(x);
        writer.Put(y);
        writer.Put(targetX);
        writer.Put(targetY);
        writer.Put(speed);
        writer.Put(Coin);
    }
    public void Deserialize(NetDataReader reader)
    {
        this.Id = reader.GetInt();
        this.Name = reader.GetString(16);
        this.x = reader.GetFloat();
        this.y = reader.GetFloat();
        this.targetX = reader.GetFloat();
        this.targetY = reader.GetFloat();
        this.speed = reader.GetFloat();
        this.Coin = reader.GetInt();
    }
}
