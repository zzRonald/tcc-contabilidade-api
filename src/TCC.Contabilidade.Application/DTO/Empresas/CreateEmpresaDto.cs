using System.ComponentModel.DataAnnotations;

namespace TCC.Contabilidade.Application.DTO.Empresas;

public class CreateEmpresaDto
{
    [Required(ErrorMessage = "É necessário informar o nome da empresa.")]
    [MinLength(2, ErrorMessage = "O nome da empresa deve ter no mínimo 2 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome da empresa deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "É necessário informar o CNPJ da empresa.")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter exatamente 14 dígitos numéricos.")]
    public string CNPJ { get; set; } = string.Empty;
}