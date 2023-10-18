

public class ArmorItemMaterial : IArmorItemMaterial
{
    public ArmorItemMaterial(string name, int armorValue, EquipmentSlot slot)
    {
        this.EquipmentSlot = slot;
        this.Name = name;
        this.Stackable = false;
        this.Armor = armorValue;
        this.Type = slot==EquipmentSlot.CHEST ? IType.ARMOR : IType.HELMET;
    }
    public EquipmentSlot EquipmentSlot { get; set; }
    public int Armor { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
}