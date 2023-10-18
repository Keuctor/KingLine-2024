
public class ItemMaterial : IItemMaterial
{
    public ItemMaterial(string name, bool stackable=true)
    {
        this.Name = name;
        this.Stackable = stackable;
        this.Type = IType.RESOURCE;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
}
