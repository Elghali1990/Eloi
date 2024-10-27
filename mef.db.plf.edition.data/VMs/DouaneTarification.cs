namespace E.Loi.Edition.Generation.VMs;

public class DouaneTarification
{

    public string version { get; set; } = default!;
    public string label { get; set; } = default!;
    public long lastModification { get; set; }
    public DouaneTarificationRow[] rows { get; set; }

}

public class DouaneTarificationRow
{
    public double? tarif { get; set; }
    public string designation { get; set; } = default!;
    public string unit { get; set; } = default!;
    public string unit_complementary { get; set; } = default!;
    public string nomenclature_level_1 { get; set; } = default!;
    public string nomenclature_level_2 { get; set; } = default!;
    public string nomenclature_level_3 { get; set; } = default!;
    public string nomenclature_level_4 { get; set; } = default!;
    public string nomenclature_level_5 { get; set; } = default!;
    public bool isDeleted { get; set; }
    public long lastModification { get; set; }
    public string uuid { get; set; } = default!;
    public bool isEmpty { get { return designation == ".........."; } }
}