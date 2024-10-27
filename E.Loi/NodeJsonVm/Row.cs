namespace E.Loi.NodeJsonVm;

public class Row
{
    public double? tarif { get; set; }
    public string designation { get; set; } = default!;

    public string unit { get; set; } = default!;
    public string unit_complementary { get; set; } = default!;
    public int lastModification { get; set; }
    public string uuid { get; set; } = default!;
    public string nomenclature_level_1 { get; set; } = default!;
    public string nomenclature_level_2 { get; set; } = default!;
    public string nomenclature_level_3 { get; set; } = default!;
    public string nomenclature_level_4 { get; set; } = default!;
    public string nomenclature_level_5 { get; set; } = default!;
}
