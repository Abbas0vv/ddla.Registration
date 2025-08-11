using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email və ya istifadəçi adı daxil edilməlidir")]
    [Display(Name = "Email və ya istifadəçi adı")]
    public string EmailOrName { get; set; }

    [Required(ErrorMessage = "Şifrə daxil edilməlidir")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifrə")]
    public string Password { get; set; }
}

