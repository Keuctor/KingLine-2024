using System;
using System.Collections.Generic;
using System.Text;

public class ReqInventory { }
public class ReqMineStone { }
public class ReqMineBone { }
public class ReqPlayerXp { }
public class ReqPlayers { }
public class ReqPlayerTeam { }
public class ReqPlayerMove
{
    public float x { get; set; }
    public float y { get; set; }
}
public class ReqStructures
{
}

public class ReqPlayerProgression
{
}
public class ReqUpgradeTeam
{
    public int MemberId { get; set; }
}

public class ReqSkillIncrement
{
    public string SkillName { get; set; }
}

public class ReqInventoryMove
{
    public short FromIndex { get; set; }
    public short ToIndex { get; set; }
}

public class ReqSellItem
{
    public int Index { get; set; }
    public short Count { get; set; }
}

public class ReqVolunteers
{
    public int StructureId { get; set; }
}