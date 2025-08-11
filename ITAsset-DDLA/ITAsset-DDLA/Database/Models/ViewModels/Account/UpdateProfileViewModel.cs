using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ddla.ITApplication.Database.Models.ViewModels.Account
{
    public class UpdateProfileViewModel
    {
        [HiddenInput]
        public string Id { get; set; }

        [Required(ErrorMessage = "Ad tələb olunur")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad tələb olunur")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "İstifadəçi adı tələb olunur")]
        [Display(Name = "İstifadəçi adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Yanlış email formatı")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Profil şəkli")]
        public IFormFile? ProfilePicture { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}