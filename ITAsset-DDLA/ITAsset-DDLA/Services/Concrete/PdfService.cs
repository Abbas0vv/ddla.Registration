using ddla.ITApplication.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ITAsset_DDLA.Services.Concrete;
public class PdfService : IPdfService
{
    private readonly IActivityLogger _activityLogger;

    public PdfService(IActivityLogger activityLogger)
    {
        _activityLogger = activityLogger;
    }

    public byte[] GenerateHandoverPdf(Transfer product, string username)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        using (var memoryStream = new MemoryStream())
        {
            // Fonts
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var regularFont = new iTextSharp.text.Font(baseFont, 12);
            var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
            var italicFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.ITALIC);

            // Document
            var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Logo
            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "ddlaLogo.png");
            if (File.Exists(logoPath))
            {
                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(50, 50);
                logo.Alignment = Element.ALIGN_LEFT;
                document.Add(logo);
            }

            // Title
            var title = new Paragraph("Dövlət Dəniz və Liman Agentliyi", boldFont) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);

            var subtitle = new Paragraph("əməkdaşa təhvil verilən avadanlıqların siyahısı", regularFont) { Alignment = Element.ALIGN_CENTER };
            document.Add(subtitle);

            document.Add(new Paragraph(" "));

            // Table
            PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 0.5f, 1f, 2f, 1f, 2f });

            string[] headers = { "№", "İnventar №", "Avadanlığın adı", "Sayı", "Əlavə qeyd" };
            foreach (var h in headers)
            {
                table.AddCell(new PdfPCell(new Phrase(h, boldFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    PaddingTop = 8,
                    PaddingBottom = 8,
                    PaddingLeft = 5,
                    PaddingRight = 5
                });
            }

            // Product row
            table.AddCell(new PdfPCell(new Phrase("1", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(product.InventarId ?? "", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(product.Name, regularFont)) { Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase("1", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(product.Description ?? "", regularFont)) { Padding = 6 });

            document.Add(table);
            document.Add(new Paragraph(" "));

            // Note
            var note = new Paragraph("*Qeyd: Təhvil alan şəxs avadanlığa qəsdən və ya ehtiyatsızlıqdan vurduğu ziyana görə maddi məsuliyyət daşıyır.", italicFont);
            document.Add(note);
            document.Add(new Paragraph(" "));

            // Signature
            PdfPTable signatureTable = new PdfPTable(1) { HorizontalAlignment = Element.ALIGN_RIGHT, WidthPercentage = 40 };

            signatureTable.AddCell(new PdfPCell(new Phrase("Təhvil alan əməkdaş:", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 6 });
            signatureTable.AddCell(new PdfPCell(new Phrase($"Soyad Ad: {product.Recipient}", regularFont)) { Padding = 6 });
            signatureTable.AddCell(new PdfPCell(new Phrase("İmza : ____________________", regularFont)) { Padding = 6 });
            signatureTable.AddCell(new PdfPCell(new Phrase($"Təhvil Verilmə Tarixi: {product.DateofIssue:dd.MM.yyyy}", regularFont)) { Padding = 6 });
            signatureTable.AddCell(new PdfPCell(new Phrase($"Təslim Alınma Tarixi: {(product.DateofReceipt?.ToString("dd.MM.yyyy") ?? "____________________")}", regularFont)) { Padding = 6 });

            document.Add(signatureTable);

            document.Close();

            _activityLogger.LogAsync(username, "Təhvil-Təslim faylını yüklədi.");

            return memoryStream.ToArray();
        }
    }
    public byte[] GenerateBlankPdf(string recipient, List<Transfer> products)
    {
        using var memoryStream = new MemoryStream();

        // Fonts
        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        var regularFont = new iTextSharp.text.Font(baseFont, 12);
        var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
        var italicFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.ITALIC);

        // Document
        var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
        PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Logo
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "ddlaLogo.png");
        if (File.Exists(logoPath))
        {
            var logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleAbsolute(50, 50);
            logo.Alignment = Element.ALIGN_LEFT;
            document.Add(logo);
        }

        // Title & Subtitle
        document.Add(new Paragraph("Dövlət Dəniz və Liman Agentliyi", boldFont) { Alignment = Element.ALIGN_CENTER });
        document.Add(new Paragraph("əməkdaşa təhvil verilən avadanlıqların siyahısı", regularFont) { Alignment = Element.ALIGN_CENTER });
        document.Add(new Paragraph(" "));

        // Table
        PdfPTable table = new PdfPTable(6) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 0.5f, 2f, 2f, 1.5f, 1.5f, 1.5f });
        string[] headers = { "№", "Avadanlığın adı", "Qeyd", "İnventar №", "Təhvil verildi", "Təhvil alındı" };
        foreach (var h in headers)
            table.AddCell(new PdfPCell(new Phrase(h, boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 8 });

        // Products
        for (int i = 0; i < 15; i++)
        {
            if (i < products.Count)
            {
                var p = products[i];
                table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                table.AddCell(new PdfPCell(new Phrase(p.Name, regularFont)) { Padding = 6 });
                table.AddCell(new PdfPCell(new Phrase(p.Description ?? "", regularFont)) { Padding = 6 });
                table.AddCell(new PdfPCell(new Phrase(p.InventarId ?? "", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                table.AddCell(new PdfPCell(new Phrase(p.DateofIssue.ToString("dd.MM.yyyy"), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                table.AddCell(new PdfPCell(new Phrase(p.DateofReceipt?.ToString("dd.MM.yyyy") ?? "", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
            }
            else
            {
                // empty row
                table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                for (int j = 0; j < 5; j++)
                    table.AddCell(new PdfPCell(new Phrase(" ", regularFont)) { Padding = 6, HorizontalAlignment = j >= 3 ? Element.ALIGN_CENTER : Element.ALIGN_LEFT });
            }
        }

        document.Add(table);
        document.Add(new Paragraph(" "));

        // Signature
        PdfPTable signatureTable = new PdfPTable(2) { WidthPercentage = 100 };
        signatureTable.SetWidths(new float[] { 1f, 1f });

        PdfPCell issuerCell = new PdfPCell(new Phrase("Təhvil verən:", boldFont)) { Border = Rectangle.NO_BORDER, Padding = 10 };
        PdfPCell receiverCell = new PdfPCell(new Phrase("Təhvil alan:", boldFont)) { Border = Rectangle.NO_BORDER, Padding = 10 };
        signatureTable.AddCell(issuerCell);
        signatureTable.AddCell(receiverCell);

        issuerCell = new PdfPCell(new Phrase("\n____________________", regularFont)) { Border = Rectangle.NO_BORDER, PaddingTop = 30, PaddingLeft = 10 };
        receiverCell = new PdfPCell(new Phrase(recipient + "\n____________________", regularFont)) { Border = Rectangle.NO_BORDER, PaddingTop = 30, PaddingLeft = 10 };
        signatureTable.AddCell(issuerCell);
        signatureTable.AddCell(receiverCell);

        issuerCell = new PdfPCell(new Phrase("(Ad, Soyad)", italicFont)) { Border = Rectangle.NO_BORDER, PaddingLeft = 10 };
        receiverCell = new PdfPCell(new Phrase("(Ad, Soyad)", italicFont)) { Border = Rectangle.NO_BORDER, PaddingLeft = 10 };
        signatureTable.AddCell(issuerCell);
        signatureTable.AddCell(receiverCell);

        document.Add(signatureTable);
        document.Add(new Paragraph(" "));

        // Note
        document.Add(new Paragraph("*Qeyd: Təhvil alan şəxs avadanlığa qəsdən və ya ehtiyatsızlıqdan vurduğu ziyana görə maddi məsuliyyət daşıyır.", italicFont));

        document.Close();
        return memoryStream.ToArray();
    }
}
