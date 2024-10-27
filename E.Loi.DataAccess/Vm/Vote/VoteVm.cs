using System.ComponentModel.DataAnnotations;

namespace E.Loi.DataAccess.Vm.Vote;

public class VoteVm
{
    [Required(ErrorMessage = "")]
    [Range(0, int.MaxValue, ErrorMessage = "المرجو إدخال عدد الموافقون")]
    public int InFavor { get; set; }
    [Required(ErrorMessage = "")]
    [Range(0, int.MaxValue, ErrorMessage = "المرجو إدخال عدد المعارضون")]
    public int Against { get; set; }
    [Required(ErrorMessage = "")]
    [Range(0, int.MaxValue, ErrorMessage = "المرجو إدخال عدد الممتنعون")]
    public int Neutral { get; set; }
    public List<Guid> Ids { get; set; }
    [Required(ErrorMessage = "المرجو إختيار نتيجة التصويت ")]
    public string? Result { get; set; }
    public string? Observation { get; set; }
    public Guid UserId { get; set; }
}
