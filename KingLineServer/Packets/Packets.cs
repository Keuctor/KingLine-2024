

using KingLineServer;
using LiteNetLib.Utils;

public partial class Player : INetSerializable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float targetX { get; set; }
    public float targetY { get; set; }
    public float speed { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(x);
        writer.Put(y);
        writer.Put(targetX);
        writer.Put(targetY);
        writer.Put(speed);
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
    }
}

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


public class ReqPlayers
{

}

public class ReqStructures
{

}
public class ReqInventory { }
public class ResInventory { 
    public ItemStack[] Items { get; set; }
}

public class ResPlayers
{
    public Player[] Players { get; set; }
}
public class ResStructures
{
    public Structure[] Structures { get; set; }
}

public class ResPlayerPosition
{
    public int Id { get; set; }
    public float x { get; set; }
    public float y { get; set; }
}

public class ResPlayerJoin
{
    public Player Player { get; set; }
}

public class ResPlayerLeave
{
    public Player Player { get; set; }
}

public class ResPeerId
{
    public int Id { get; set; }
}

public class ReqPlayerMove
{
    public float x { get; set; }
    public float y { get; set; }
}

public class ResPlayerMove
{
    public int Id { get; set; }
    public float y { get; set; }
    public float x { get; set; }
}
