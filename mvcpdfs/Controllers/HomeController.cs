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
                        col.Item().Text($"Nome do cliente: {model.FullName}");
                        col.Item().Text($"Data: {DateTime.Now:dd/MM/yyyy}");
                        col.Item().Text($"Número: {Guid.NewGuid().ToString()[..8]}");

                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Descrição");
                                header.Cell().Element(CellStyle).AlignRight().Text("Valor");
                            });

                            for (int i = 1; i <= 3; i++)
                            {
                                table.Cell().Element(CellStyle).Text(i.ToString());
                                table.Cell().Element(CellStyle).Text("Item de teste");
                                table.Cell().Element(CellStyle).AlignRight().Text($"R$ {(model.Amount/3):F2}");
                            }

                            static IContainer CellStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1);
                        col.Item().AlignRight().Text($"Total: R$ {model.Amount:F2}").Bold();
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
