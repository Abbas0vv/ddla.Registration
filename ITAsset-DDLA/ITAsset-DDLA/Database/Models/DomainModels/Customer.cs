using System.ComponentModel.DataAnnotations;

namespace ITAsset_DDLA.Database.Models.DomainModels;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public string Email { get; set; }
}
