using ClosedXML.Excel;
using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.LDAP;
using ITAsset_DDLA.Services.Abstract;

namespace ITAsset_DDLA.Services.Concrete;

public class ExcelService : IExcelService
{
    public byte[] ExportProductsToExcel(List<Transfer> products)
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
    public byte[] ExportLogsToExcel(List<ActivityLog> logs)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Activity Logs");
        int currentRow = 1;

        // Header
        string[] headers = { "İstifadəçi", "Əməliyyat", "Tarix" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(currentRow, i + 1).Value = headers[i];
        }

        // Data
        foreach (var log in logs)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = log.UserFullName;
            worksheet.Cell(currentRow, 2).Value = log.Action;
            worksheet.Cell(currentRow, 3).Value = log.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    public byte[] ExportUsersToExcel(List<LdapUserModel> users)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("İstifadəçilər");
        int currentRow = 1;

        // Header
        string[] headers = { "Tam ad", "E-poçt", "Telefon", "Şöbə", "Vəzifə" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(currentRow, i + 1).Value = headers[i];
            worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
        }

        // Data (FullName-ə görə sıralama)
        foreach (var user in users.OrderBy(u => u.FullName))
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = user.FullName ?? "";
            worksheet.Cell(currentRow, 2).Value = user.Email ?? "";
            worksheet.Cell(currentRow, 3).Value = user.InternalPhone ?? "";
            worksheet.Cell(currentRow, 4).Value = user.Shobe ?? "";
            worksheet.Cell(currentRow, 5).Value = user.Vazifa ?? "";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

}