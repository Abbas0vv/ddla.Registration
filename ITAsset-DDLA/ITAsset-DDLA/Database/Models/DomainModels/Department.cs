namespace ddla.ITApplication.Database.Models.DomainModels;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
}
