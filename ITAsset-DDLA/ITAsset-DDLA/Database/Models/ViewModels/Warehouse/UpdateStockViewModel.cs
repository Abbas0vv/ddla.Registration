using ITAsset_DDLA.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Warehouse;

public class UpdateStockViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Məhsul adı mütləq doldurulmalıdır")]
    [Display(Name = "Məhsul Adı")]
    [StringLength(100, ErrorMessage = "Məhsul adı maksimum 100 simvol ola bilər")]
    public string Name { get; set; }

    [Display(Name = "Təsvir")]
    [StringLength(500, ErrorMessage = "Təsvir maksimum 500 simvol ola bilər")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Departament seçilməlidir")]
    [Display(Name = "Departament")]
    public DepartmentName DepartmentName { get; set; }

    [Required(ErrorMessage = "Şöbə seçilməlidir")]
    [Display(Name = "Şöbə")]
    public UnitName UnitName { get; set; }

    [Required(ErrorMessage = "Sayı mütləq doldurulmalıdır")]
    [Display(Name = "Sayı")]
    [Range(1, int.MaxValue, ErrorMessage = "Sayı 1-dən böyük olmalıdır")]
    public int TotalCount { get; set; }

    [Display(Name = "Şəkil")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Cari Sənəd")]
    public string? DocumentPath { get; set; }

    [Display(Name = "Cari Şəkil")]
    public string? ImagePath { get; set; }

    [Display(Name = "Qeydiyyat Tarixi")]
    public DateTime? DateofRegistration { get; set; }
}