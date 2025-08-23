using System.ComponentModel.DataAnnotations;
using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class UpdateTransferProductViewModel
{
    [Required(ErrorMessage = "Məhsulu təhvil alan mütləq olmalıdır")]
    [Display(Name = "Məhsulu Təhvil Alan")]
    public string Recipient { get; set; }

    [Required(ErrorMessage = "Departament seçilməlidir")]
    [Display(Name = "Departamentə")]
    public string DepartmentSection { get; set; }

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Alınma Tarixi")]
    public DateTime? DateofReceipt { get; set; }

    [Required(ErrorMessage = "Məhsul seçilməlidir")]
    [Display(Name = "Məhsul")]
    public int StockProductId { get; set; }
}