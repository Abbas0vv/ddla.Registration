using ClosedXML.Excel;
using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;

namespace ITAsset_DDLA.Services.Concrete;

public class ExcelService : IExcelService
{
    public byte[] ExportProductsToExcel(List<Product> products)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Təhvil-Təslim Jurnalı");
        int currentRow = 1;

        // Header
        string[] headers = { "İmzalanma Statusu", "İnventar ID", "Təhvil Alan", "Məhsul", "Təsvir", "Departament", "Bölmə", "Verilmə tarixi", "Alınma tarixi" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(currentRow, i + 1).Value = headers[i];
        }

        // Data
        foreach (var p in products)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = p.IsSigned ? "İmzalanıb" : "İmzalanmayıb";
            worksheet.Cell(currentRow, 2).Value = p.InventarId;
            worksheet.Cell(currentRow, 3).Value = p.Recipient;
            worksheet.Cell(currentRow, 4).Value = p.StockProduct?.Name ?? "";
            worksheet.Cell(currentRow, 5).Value = p.StockProduct?.Description ?? "";
            worksheet.Cell(currentRow, 6).Value = p.DepartmentSection ?? "";
            worksheet.Cell(currentRow, 8).Value = p.DateofIssue.ToString("dd.MM.yyyy");
            worksheet.Cell(currentRow, 9).Value = p.DateofReceipt?.ToString("dd.MM.yyyy");
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}