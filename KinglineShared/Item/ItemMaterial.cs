
public class ItemMaterial : IItemMaterial
{
    public ItemMaterial(string name,string type, bool stackable=true)
    {
        this.Name = name;
        this.Stackable = stackable;
        this.Type = type;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public string Type { get; set; }
}
