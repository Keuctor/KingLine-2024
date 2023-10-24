

public class ArmorItemMaterial : IArmorItemMaterial
{
    public ArmorItemMaterial(string name, int armorValue, EquipmentSlot slot)
    {
        this.EquipmentSlot = slot;
        this.Name = name;
        this.Stackable = false;
        this.Armor = armorValue;
        this.Type = slot==EquipmentSlot.ARMOR ? IType.ARMOR : IType.HELMET;
        this.Value = Armor * 20;
    }
    public EquipmentSlot EquipmentSlot { get; set; }
    public int Armor { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
    public int Value { get; set; }
}