using ITAsset_DDLA.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class CreateProductViewModel
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

    [Required(ErrorMessage = "Ən azı bir məhsul seçilməlidir")]
    [Display(Name = "Məhsullar")]
    public List<int> StockProductIds { get; set; } = new List<int>();
}