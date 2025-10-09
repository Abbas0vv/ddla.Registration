using System.ComponentModel.DataAnnotations;

namespace ITAsset_DDLA.Database.Models.ViewModels.Warehouse;

public class CreateProductFilesViewModel
{
    [Display(Name = "İmzalanmış Məhsul Aktı")]
    public IFormFile? SignedFile { get; set; }
    [Display(Name = "Qaytarılmış Məhsul Aktı")]
    public IFormFile? ReturnedFile { get; set; }
}
