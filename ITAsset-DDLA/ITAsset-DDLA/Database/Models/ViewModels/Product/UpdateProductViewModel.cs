using System.ComponentModel.DataAnnotations;
using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class UpdateProductViewModel
{
    [Required(ErrorMessage = "Məhsulu təhvil alan mütləq olmalıdır")]
    [Display(Name = "Məhsulu Təhvil Alan")]
    [StringLength(100, ErrorMessage = "Məhsulu təhvil alan maksimum 100 simvol ola bilər")]
    public string Recipient { get; set; }

    [Required(ErrorMessage = "Departament seçilməlidir")]
    [Display(Name = "Departament")]
    public DepartmentName DepartmentName { get; set; }

    [Required(ErrorMessage = "Şöbə seçilməlidir")]
    [Display(Name = "Şöbə")]
    public UnitName UnitName { get; set; }

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Alınma Tarixi")]
    public DateTime? DateofReceipt { get; set; }

    [Required(ErrorMessage = "Məhsul seçilməlidir")]
    [Display(Name = "Məhsul")]
    public int StockProductId { get; set; }
}