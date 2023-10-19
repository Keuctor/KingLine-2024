
public class ResourceItemMaterial : IItemMaterial
{
    public ResourceItemMaterial(string name, bool stackable=true)
    {
        this.Name = name;
        this.Stackable = stackable;
        this.Type = IType.RESOURCE;
        this.Value = 2;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
    public int Value { get; set; }
}
