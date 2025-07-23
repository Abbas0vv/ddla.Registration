using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Warehouse;

public class CreateStockViewModel
{
    [Required(ErrorMessage = "Məhsulu təhvil alan mütləq olmalıdır")]
    [Display(Name = "Məhsulu Təhvil Alan")]
    [StringLength(100, ErrorMessage = "Məhsulu təhvil alan maksimum 100 simvol ola bilər")]
    public string Recipient { get; set; }

    [Required(ErrorMessage = "Məhsul adı mütləq doldurulmalıdır")]
    [Display(Name = "Məhsul Adı")]
    [StringLength(100, ErrorMessage = "Məhsul adı maksimum 100 simvol ola bilər")]
    public string Name { get; set; }

    [Display(Name = "Təsvir")]
    [StringLength(500, ErrorMessage = "Təsvir maksimum 500 simvol ola bilər")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Departament seçilməlidir")]
    [Display(Name = "Departament")]
    public string DepartmentName { get; set; }

    [Required(ErrorMessage = "Şöbə seçilməlidir")]
    [Display(Name = "Şöbə")]
    public string UnitName { get; set; }

    [Required(ErrorMessage = "Sayı mütləq doldurulmalıdır")]
    [Display(Name = "Sayı")]
    [Range(1, int.MaxValue, ErrorMessage = "Sayı 1-dən böyük olmalıdır")]
    public int Count { get; set; }
    [Display(Name = "Şəkil")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Alınma Tarixi")]
    public DateTime? DateofReceipt { get; set; }

    public string DateofReceiptFormatted => DateofReceipt?.ToString("dd-MM-yyyy");
}
