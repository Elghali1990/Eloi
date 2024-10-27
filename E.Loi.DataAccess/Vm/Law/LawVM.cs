﻿namespace E.Loi.DataAccess.Vm.Law;

public class LawVM
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public string Label { get; set; } = default!;
    public string Number { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string Type { get; set; } = default!;

    public string NodeHierarchyFamillyId { get; set; } = default!;
}
