using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using mvcpdfs.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace mvcpdfs.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Invoice()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Invoice(InvoiceModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var document = CreateInvoiceDocument(model);
        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/pdf", "fatura.pdf");
    }

    private Document CreateInvoiceDocument(InvoiceModel model)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Header()
                    .Text("Fatura")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Column(col =>
                    {
                        col.Item().Text($"Nome: {model.FullName}");
                        col.Item().Text($"Valor: R$ {model.Amount:F2}");
                        col.Item().Text($"Data: {DateTime.Now:dd/MM/yyyy}");
                        col.Item().Text($"Número: {Guid.NewGuid().ToString()[..8]}");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Obrigado pela preferência.");
            });
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
