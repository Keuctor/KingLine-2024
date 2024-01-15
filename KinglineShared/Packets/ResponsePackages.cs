
using System.Security.Cryptography.X509Certificates;

public class ResPeerId
{
    public int Id { get; set; }
}

public class ResPlayerXp
{
    public int Xp { get; set; }
}

public class ResRemoteInventory
{
    public ulong Id { get; set; }
    public ItemStack[] Gear { get; set; }
}


public class ResPlayerAddXp
{
    public int Xp { get; set; }
}


public class ResPlayerProgression
{
    public Skill[] Skills { get; set; }
}

public class ResUpgradeTeam
{
    public bool Success { get; set; }
}

public class ResSkillValueChange
{
    public string SkillName { get; set; }
    public byte Value { get; set; }
}



public class ResPlayerMove
{
    public int Id { get; set; }
    public float y { get; set; }
    public float x { get; set; }
}


public class ResInventoryAdd
{
    public ulong InventoryId { get; set; }
    public int Id { get; set; }
    public short Count { get; set; }
}
public class ResInventoryRemove
{
    public ulong InventoryId { get; set; }
    public int Index { get; set; }
    public short Count { get; set; }
}



public class ResInventory
{
    public ulong Id { get; set; }
    public ItemStack[] Items { get; set; }
    public ItemStack[] Gear { get; set; }
}

public class ResStructureInventory
{
    public ulong InventoryId { get; set; }
    public int StructureId { get; set; }
    public ItemStack[] Items { get; set; }
}

public class ResStructures
{
    public Structure[] Structures { get; set; }
}

public class ResPlayers
{
    public Player[] Players { get; set; }
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

public class ResPlayerTeam
{
    public Team[] Teams { get; set; }
}

public class ResUpdatePlayerTeam
{
    public Team Team { get; set; }
}

public class ResPlayerCurrency
{

    public int NewCurrency { get; set; }
}

public class ResVolunteers
{
    /// <summary>
    /// Place ID
    /// </summary>
    public int TroopId { get; set; }
    public short Count { get; set; }
    public int StructureId { get; set; }
}

public class ResAdminPrivileges
{
    public bool IsAdmin { get; set; }
}

public class ResConsoleLog
{
    public string Log { get; set; }
}


