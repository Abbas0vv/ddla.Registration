using System.ComponentModel.DataAnnotations;

namespace ITAsset_DDLA.Database.Models.ViewModels.Account;
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Cari şifrə daxil edin")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifrə daxil edin")]
    [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifrə təkrarını daxil edin")]
    [Compare("NewPassword", ErrorMessage = "Şifrələr uyğun deyil")]
    public string ConfirmPassword { get; set; }
}