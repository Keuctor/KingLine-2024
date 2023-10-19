
public interface IItemMaterial
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable{ get; set; }
    public IType Type { get; set; }
    public int Value { get; set; }
}
