using ddla.ITApplication.Services.Abstract;

namespace ddla.ITApplication.Helpers;

public static class ConverterExtention
{
    public static string ConvertToNameById(int id, IProductService productService)
    {
        return productService.GetByIdAsync(id)
            .Result?.Name ?? string.Empty;
    }

        public static int ConvertToIdByName(string name, IProductService productService)
    {
        return productService.GetAllAsync()
            .Result.FirstOrDefault(p => p.Name == name)?.Id ?? 0;
    }
}
