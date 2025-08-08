using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Services.Abstract;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ddlaAppDBContext _context;

    public HomeController(
        IProductService productService,
        IWebHostEnvironment webHostEnvironment,
        IStockService stockService,
        ddlaAppDBContext context)
    {
        _productService = productService;
        _webHostEnvironment = webHostEnvironment;
        _stockService = stockService;
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        var models = await _productService.GetAllAsync();
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new DoubleCreateProductTypeViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            StockProducts = await _context.StockProducts.ToListAsync()
        };
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(DoubleCreateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.StockProducts = await _context.StockProducts.ToListAsync();
            return View(model);
        }


        await _productService.InsertMultipleAsync(model);

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var stockProduct = await _stockService.GetByIdAsync(product.StockProductId);

        var model = new DoubleUpdateProductTypeViewModel
        {
            UpdateProductViewModel = new UpdateProductViewModel
            {
                Recipient = product.Recipient,
                DepartmentName = product.Department,
                UnitName = product.Unit,
                DateofReceipt = product.DateofReceipt,
                StockProductId = product.StockProductId
            },
            StockProduct = stockProduct
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(DoubleUpdateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _productService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        await _productService.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult GenerateHandoverPdf(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        var regularFont = new iTextSharp.text.Font(baseFont, 12);
        var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
        var italicFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.ITALIC);

        var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
        var memoryStream = new MemoryStream();
        var writer = PdfWriter.GetInstance(document, memoryStream);

        document.Open();

        // Loqo əlavə et (sol yuxarı)
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "ddlaLogo.png");
        if (System.IO.File.Exists(logoPath))
        {
            var logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleAbsolute(50, 50);
            logo.Alignment = Element.ALIGN_LEFT;
            document.Add(logo);
        }


        // Başlıq
        var title = new Paragraph("Dövlət Dəniz və Liman Agentliyi", boldFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        var subtitle = new Paragraph("əməkdaşa təhvil verilən avadanlıqların siyahısı", regularFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);

        document.Add(new Paragraph(" "));

        // Cədvəl
        PdfPTable table = new PdfPTable(4);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 3f, 1f, 3f });

        // Başlıqlar
        string[] headers = { "№", "Avadanlığın adı", "Sayı", "Əlavə qeyd" };
        foreach (var h in headers)
        {
            var cell = new PdfPCell(new Phrase(h, boldFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                PaddingTop = 8,
                PaddingBottom = 8,
                PaddingLeft = 5,
                PaddingRight = 5
            };
            table.AddCell(cell);
        }

        // 1-ci sətir məhsul
        table.AddCell(new PdfPCell(new Phrase("1", regularFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 6
        });

        table.AddCell(new PdfPCell(new Phrase(product.Name, regularFont))
        {
            Padding = 6
        });

        table.AddCell(new PdfPCell(new Phrase("1", regularFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 6
        });

        table.AddCell(new PdfPCell(new Phrase(product.Description ?? "", regularFont))
        {
            Padding = 6
        });


        // Qalan boş sətirlər
        for (int i = 2; i <= 5; i++)
        {
            table.AddCell(new PdfPCell(new Phrase(i.ToString(), regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6

            });
            table.AddCell(new PdfPCell(new Phrase(" ", regularFont))
            {
                Padding = 6
            });
            table.AddCell(new PdfPCell(new Phrase(" ", regularFont)) 
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });
            table.AddCell(new PdfPCell(new Phrase(" ", regularFont)) 
            {
                Padding = 6
            });
        }

        document.Add(table);

        document.Add(new Paragraph(" "));

        // Qeyd
        Paragraph note = new Paragraph("*Qeyd: Təhvil alan şəxs avadanlığa qəsdən və ya ehtiyatsızlıqdan vurduğu ziyana görə maddi məsuliyyət daşıyır.", italicFont);
        document.Add(note);

        document.Add(new Paragraph(" "));

        // Təhvil aldı qutusu
        PdfPTable signatureTable = new PdfPTable(1);
        signatureTable.HorizontalAlignment = Element.ALIGN_RIGHT;
        signatureTable.WidthPercentage = 40;

        PdfPCell headerCell = new PdfPCell(new Phrase("Təhvil aldı:", boldFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = BaseColor.LIGHT_GRAY,
            Padding = 6
        };
        signatureTable.AddCell(headerCell);

        signatureTable.AddCell(new PdfPCell(new Phrase($"Soyad Ad: {product.Recipient}", regularFont))
        {
            Padding = 6
        });
        signatureTable.AddCell(new PdfPCell(new Phrase("İmza : ____________________", regularFont))
        {
            Padding = 6
        });
        signatureTable.AddCell(new PdfPCell(new Phrase($"Tarix: {DateTime.Now:dd.MM.yyyy}", regularFont))
        {
            Padding = 6
        });


        document.Add(signatureTable);

        document.Close();

        return File(memoryStream.ToArray(), "application/pdf", $"TehvilTeslim_{product.InventarId}.pdf");
    }

}
