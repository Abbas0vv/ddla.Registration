using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels;
using ddla.ITApplication.Database.Models.ViewModels.Product;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Shared;
using ITAsset_DDLA.Helpers.Attributes;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using ITAsset_DDLA.Services.Concrete;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ddla.ITApplication.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly IActivityLogger _activityLogger;
    private readonly LdapService _ldapService;
    private readonly ddlaAppDBContext _context;

    public HomeController(
        IProductService productService,
        IStockService stockService,
        ddlaAppDBContext context,
        LdapService ldapService,
        IActivityLogger activityLogger)
    {
        _productService = productService;
        _stockService = stockService;
        _context = context;
        _ldapService = ldapService;
        _activityLogger = activityLogger;
    }

    [Permission(PermissionType.OperationView)]
    public async Task<IActionResult> Index()
    {
        var models = await _productService.GetAllAsync();
        return View(models);
    }


    [Permission(PermissionType.OperationAdd)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var ldapUsers = _ldapService.GetLdapUsers();
        ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();

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
            var ldapUsers = _ldapService.GetLdapUsers();
            ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();

            model = new DoubleCreateProductTypeViewModel
            {
                CreateProductViewModel = new CreateProductViewModel(),
                StockProducts = await _context.StockProducts.ToListAsync()
            };
            return View(model);
        }

        await _activityLogger.LogAsync(User.Identity.Name, "yeni məhsul əlavə etdi");
        await _productService.InsertMultipleAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [Permission(PermissionType.OperationEdit)]
    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        var ldapUsers = _ldapService.GetLdapUsers();
        ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();

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
            await _activityLogger.LogAsync(User.Identity.Name, "məhsulu redaktə etdi");
            await _productService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
            return View(model);
        }
    }

    [Permission(PermissionType.OperationDelete)]
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

        // Logo
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "ddlaLogo.png");
        if (System.IO.File.Exists(logoPath))
        {
            var logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleAbsolute(50, 50);
            logo.Alignment = Element.ALIGN_LEFT;
            document.Add(logo);
        }

        // Title
        var title = new Paragraph("Dövlət Dəniz və Liman Agentliyi", boldFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        var subtitle = new Paragraph("əməkdaşa təhvil verilən avadanlıqların siyahısı", regularFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);

        document.Add(new Paragraph(" "));

        // Table (now with 5 columns)
        PdfPTable table = new PdfPTable(5);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1f, 2f, 1f, 2f });

        // Headers (updated with inventory number column)
        string[] headers = { "№", "İnventar №", "Avadanlığın adı", "Sayı", "Əlavə qeyd" };
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

        // Product row (only row)
        table.AddCell(new PdfPCell(new Phrase("1", regularFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 6
        });

        // Inventory number
        table.AddCell(new PdfPCell(new Phrase(product.InventarId ?? "", regularFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 6
        });

        // Product name
        table.AddCell(new PdfPCell(new Phrase(product.Name, regularFont))
        {
            Padding = 6
        });

        // Quantity
        table.AddCell(new PdfPCell(new Phrase("1", regularFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 6
        });

        // Description
        table.AddCell(new PdfPCell(new Phrase(product.Description ?? "", regularFont))
        {
            Padding = 6
        });

        document.Add(table);

        document.Add(new Paragraph(" "));

        // Note
        Paragraph note = new Paragraph("*Qeyd: Təhvil alan şəxs avadanlığa qəsdən və ya ehtiyatsızlıqdan vurduğu ziyana görə maddi məsuliyyət daşıyır.", italicFont);
        document.Add(note);

        document.Add(new Paragraph(" "));

        // Signature box
        PdfPTable signatureTable = new PdfPTable(1);
        signatureTable.HorizontalAlignment = Element.ALIGN_RIGHT;
        signatureTable.WidthPercentage = 40;

        PdfPCell headerCell = new PdfPCell(new Phrase("Təhvil alan əməkdaş:", boldFont))
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
        signatureTable.AddCell(new PdfPCell(new Phrase($"Verilmə Tarixi: {product.DateofIssue:dd.MM.yyyy}", regularFont))
        {
            Padding = 6
        });
        signatureTable.AddCell(new PdfPCell(new Phrase(
            $"Alınma Tarixi: {(product.DateofReceipt?.ToString("dd.MM.yyyy") ?? "____________________")}", regularFont))
        {
            Padding = 6
        });

        document.Add(signatureTable);

        document.Close();

        product.IsSigned = true;
        _context.SaveChanges();

        _activityLogger.LogAsync(User.Identity.Name, "Təhvil-Təslim faylını yüklədi.");
        return File(memoryStream.ToArray(), "application/pdf", $"TehvilTeslim_{product.InventarId}.pdf");
    }


    [Permission(PermissionType.OperationAdd)]
    [HttpGet]
    public async Task<IActionResult> CreateBlank()
    {
        var ldapUsers = _ldapService.GetLdapUsers();
        ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();

        var model = new DoubleCreateProductTypeViewModel
        {
            CreateProductViewModel = new CreateProductViewModel(),
            StockProducts = await _context.StockProducts.ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlank(DoubleCreateProductTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var ldapUsers = _ldapService.GetLdapUsers();
            ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();
            return View(model);
        }

        // Get all products for the selected user
        var userProducts = await _context.Products
            .Where(p => p.Recipient == model.CreateProductViewModel.Recipient)
            .ToListAsync();

        if (!userProducts.Any())
        {
            ModelState.AddModelError("", "Seçilmiş istifadəçiyə aid məhsul tapılmadı.");
            var ldapUsers = _ldapService.GetLdapUsers();
            ViewBag.LdapUsers = ldapUsers.Select(u => u.FullName).ToList();
            return View(model);
        }

        // Generate the blank PDF
        await _activityLogger.LogAsync(User.Identity.Name,$"{model.CreateProductViewModel.Recipient} üçün ümumi akt yaratdı");
        return GenerateBlankPdf(model.CreateProductViewModel.Recipient, userProducts);
    }

    private IActionResult GenerateBlankPdf(string recipient, List<Product> products)
    {
        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        var regularFont = new iTextSharp.text.Font(baseFont, 12);
        var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
        var italicFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.ITALIC);

        var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
        var memoryStream = new MemoryStream();
        var writer = PdfWriter.GetInstance(document, memoryStream);

        document.Open();

        // Logo
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "ddlaLogo.png");
        if (System.IO.File.Exists(logoPath))
        {
            var logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleAbsolute(50, 50);
            logo.Alignment = Element.ALIGN_LEFT;
            document.Add(logo);
        }

        // Title
        var title = new Paragraph("Dövlət Dəniz və Liman Agentliyi", boldFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        var subtitle = new Paragraph("əməkdaşa təhvil verilən avadanlıqların siyahısı", regularFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);

        document.Add(new Paragraph(" "));

        document.Add(new Paragraph(" "));

        // Table with 6 columns
        PdfPTable table = new PdfPTable(6);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 2f, 2f, 1.5f, 1.5f, 1.5f });

        // Headers
        string[] headers = { "№", "Avadanlığın adı", "Qeyd", "İnventar №", "Təhvil verildi", "Təhvil alındı" };
        foreach (var h in headers)
        {
            var cell = new PdfPCell(new Phrase(h, boldFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                Padding = 8
            };
            table.AddCell(cell);
        }

        // Add products
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            // Serial number
            table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });

            // Product name
            table.AddCell(new PdfPCell(new Phrase(product.Name, regularFont))
            {
                Padding = 6
            });

            // Note
            table.AddCell(new PdfPCell(new Phrase(product.Description ?? "", regularFont))
            {
                Padding = 6
            });

            // Inventory number
            table.AddCell(new PdfPCell(new Phrase(product.InventarId ?? "", regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });

            // Issuance (empty)
            table.AddCell(new PdfPCell(new Phrase(
                product.DateofIssue.ToString("dd.MM.yyyy") ?? "",
                regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });


            // Return (empty)
            table.AddCell(new PdfPCell(new Phrase(
                product.DateofReceipt?.ToString("dd.MM.yyyy") ?? "",
                regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });
        }

        // Fill remaining rows (up to 15)
        for (int i = products.Count; i < 15; i++)
        {
            // Serial number
            table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), regularFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            });

            // Empty cells for other columns
            for (int j = 0; j < 5; j++)
            {
                table.AddCell(new PdfPCell(new Phrase(" ", regularFont))
                {
                    Padding = 6,
                    HorizontalAlignment = j >= 3 ? Element.ALIGN_CENTER : Element.ALIGN_LEFT
                });
            }
        }

        document.Add(table);

        document.Add(new Paragraph(" "));

        // Signature section
        PdfPTable signatureTable = new PdfPTable(2);
        signatureTable.WidthPercentage = 100;
        signatureTable.SetWidths(new float[] { 1f, 1f });

        // Issuer signature
        PdfPCell issuerCell = new PdfPCell(new Phrase("Təhvil verən:", boldFont))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 10
        };
        signatureTable.AddCell(issuerCell);

        // Receiver signature
        PdfPCell receiverCell = new PdfPCell(new Phrase("Təhvil alan:", boldFont))
        {
            Border = Rectangle.NO_BORDER,
            Padding = 10
        };
        signatureTable.AddCell(receiverCell);

        // Empty space for issuer signature
        issuerCell = new PdfPCell(new Phrase("\n____________________", regularFont))
        {
            Border = Rectangle.NO_BORDER,
            PaddingTop = 30,
            PaddingLeft = 10
        };
        signatureTable.AddCell(issuerCell);

        // Empty space for receiver signature
        receiverCell = new PdfPCell(new Phrase(recipient + "\n____________________", regularFont))
        {
            Border = Rectangle.NO_BORDER,
            PaddingTop = 30,
            PaddingLeft = 10
        };
        signatureTable.AddCell(receiverCell);

        // Issuer name
        issuerCell = new PdfPCell(new Phrase("(Ad, Soyad)", italicFont))
        {
            Border = Rectangle.NO_BORDER,
            PaddingLeft = 10
        };
        signatureTable.AddCell(issuerCell);

        // Receiver name
        receiverCell = new PdfPCell(new Phrase($"(Ad,Soyad)", italicFont))
        {
            Border = Rectangle.NO_BORDER,
            PaddingLeft = 10
        };
        signatureTable.AddCell(receiverCell);

        document.Add(signatureTable);

        document.Add(new Paragraph(" "));

        // Note
        Paragraph note = new Paragraph("*Qeyd: Təhvil alan şəxs avadanlığa qəsdən və ya ehtiyatsızlıqdan vurduğu ziyana görə maddi məsuliyyət daşıyır.", italicFont);
        document.Add(note);

        document.Close();

        return File(memoryStream.ToArray(), "application/pdf", $"TehvilTeslim_Blank_{recipient}.pdf");
    }
}
