using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Product;

public class CreateProductViewModel
{
    [Required(ErrorMessage = "Məhsulu təhvil alan mütləq olmalıdır")]
    [Display(Name = "Məhsulu Təhvil Alan")]
    [StringLength(100, ErrorMessage = "Məhsulu təhvil alan maksimum 100 simvol ola bilər")]
    public string Recipient { get; set; }

    [Display(Name = "Select Inventory Items")]
    public List<int> SelectedInventoryItemIds { get; set; } = new();
    public List<InventoryItem> AvailableInventoryItems { get; set; } = new();

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

    [Display(Name = "Fayl")]
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Alınma Tarixi")]
    public DateTime? DateofReceipt { get; set; }

    // Add StockProductId to link to warehouse item
    [Required(ErrorMessage = "Məhsul seçilməlidir")]
    [Display(Name = "Anbar Məhsulu")]
    public List<int> StockProductIds { get; set; }

    // Add property to display available count (readonly)
    [Display(Name = "Mövcud Sayı")]
    public int AvailableCount { get; set; }
}