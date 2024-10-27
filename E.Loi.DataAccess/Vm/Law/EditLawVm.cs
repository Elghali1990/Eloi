using System.ComponentModel.DataAnnotations;

namespace E.Loi.DataAccess.Vm.Law;
public class EditLawVm
{
    [Required(ErrorMessage = "المرجو إدخال العنوان")]
    public string Title { get; set; } = default!;
    [Required(ErrorMessage = "")]
    [Range(1, int.MaxValue, ErrorMessage = "المرجو إدخال السنة ")]
    public int Year { get; set; }
    [Required(ErrorMessage = "المرجو إدخال الرقم ")]
    public string Number { get; set; } = default!;
    public string Category { get; set; } = default!;
    public Guid PhaseId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid TeamId { get; set; }
}
