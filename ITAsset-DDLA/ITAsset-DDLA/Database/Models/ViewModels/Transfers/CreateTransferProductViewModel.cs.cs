using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class CreateTransferProductViewModel
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

    [Required(ErrorMessage = "Ən azı bir məhsul seçilməlidir")]
    [Display(Name = "Məhsullar")]
    public List<int> StockProductIds { get; set; } = new List<int>();
}