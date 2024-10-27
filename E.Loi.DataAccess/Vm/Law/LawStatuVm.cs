using System.ComponentModel.DataAnnotations;

namespace E.Loi.DataAccess.Vm.Law;

public class LawStatuVm
{
    public Guid LawId { get; set; }

    [Required]
    [RegularExpression("^((?!00000000-0000-0000-0000-000000000000).)*$", ErrorMessage = "المرجو إختيار الوضعية")]
    public Guid StatuLaw { get; set; }
    public Guid LastModifiedBy { get; set; }
    [Required(ErrorMessage = "المرجو إدخال التاريخ")]
    public DateTime? DateSetStatu { get; set; }
}
