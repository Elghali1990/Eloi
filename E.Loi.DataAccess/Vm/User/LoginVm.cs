using System.ComponentModel.DataAnnotations;

namespace E.Loi.DataAccess.Vm.User;

public class LoginVm
{
    [Required(ErrorMessage = "المرجو إدخال أسم المستعمل")]
    public string UserName { get; set; } = default!;
    [Required(ErrorMessage = "المرجو إدخال كلمة المرور")]
    public string Password { get; set; } = default!;
}
