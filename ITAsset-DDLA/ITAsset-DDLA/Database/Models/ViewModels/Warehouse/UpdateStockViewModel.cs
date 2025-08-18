using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Warehouse
{
    public class UpdateStockViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Məhsul adı tələb olunur.")]
        [StringLength(100, ErrorMessage = "Məhsul adı maksimum 100 simvol ola bilər.")]
        [Display(Name = "Məhsul adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "İnventar kodu tələb olunur.")]
        [StringLength(50, ErrorMessage = "İnventar kodu maksimum 50 simvol ola bilər.")]
        [Display(Name = "İnventar kodu")]
        public string InventoryCode { get; set; }

        [StringLength(500, ErrorMessage = "Təsvir maksimum 500 simvol ola bilər.")]
        [Display(Name = "Təsvir")]
        public string Description { get; set; }
    }
}
