namespace mvcpdfs.Models;

using System.ComponentModel.DataAnnotations;

public class InvoiceModel
{
    [Required]
    [Display(Name = "Nome completo")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Valor inválido")]
    [Display(Name = "Valor (R$)")]
    public decimal Amount { get; set; }
}
