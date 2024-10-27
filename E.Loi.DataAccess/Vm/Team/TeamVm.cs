using System.ComponentModel.DataAnnotations;

namespace E.Loi.DataAccess.Vm.Team;

public class TeamVm
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "المرجو إدخال إسم الفريق")]
    public string Label { get; set; } = string.Empty;
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "المرجو إدخال ترتيب الفريق")]
    public int Order { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "المرجو إدخال وزن الفريق")]
    public int Weight { get; set; }
    public bool IsMajority { get; set; }
    public string? TeamType { get; set; }
}
