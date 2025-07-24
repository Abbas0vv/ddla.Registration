using System.ComponentModel.DataAnnotations;
using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class CreateProductViewModel
{
    [Required(ErrorMessage = "Məhsulu təhvil alan mütləq olmalıdır")]
    [Display(Name = "Məhsulu Təhvil Alan")]
    [StringLength(100, ErrorMessage = "Məhsulu təhvil alan maksimum 100 simvol ola bilər")]
    public string Recipient { get; set; }

    [Display(Name = "Təsvir")]
    [StringLength(500, ErrorMessage = "Təsvir maksimum 500 simvol ola bilər")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Departament seçilməlidir")]
    [Display(Name = "Departament")]
    public DepartmentName DepartmentName { get; set; } // Changed to enum type

    [Required(ErrorMessage = "Şöbə seçilməlidir")]
    [Display(Name = "Şöbə")]
    public UnitName UnitName { get; set; } // Changed to enum type

    [Required(ErrorMessage = "Sayı mütləq doldurulmalıdır")]
    [Display(Name = "Sayı")]
    [Range(1, int.MaxValue, ErrorMessage = "Sayı 1-dən böyük olmalıdır")]
    public int Count { get; set; }

    [Display(Name = "Şəkil")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Cari Sənəd")]
    public string? DocumentPath { get; set; }

    [Display(Name = "Cari Şəkil")]
    public string? ImagePath { get; set; }

    [Display(Name = "Alınma Tarixi")]
    public DateTime? DateofReceipt { get; set; }

    // Add StockProductId to link to warehouse item
    [Required(ErrorMessage = "Məhsul seçilməlidir")]
    [Display(Name = "Anbar Məhsulu")]
    public int StockProductId { get; set; }

    // Add property to display available count (readonly)
    [Display(Name = "Mövcud Sayı")]
    public int AvailableCount { get; set; }
}