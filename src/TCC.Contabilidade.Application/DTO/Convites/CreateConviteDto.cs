using System.ComponentModel.DataAnnotations;

namespace TCC.Contabilidade.Application.DTOs.Convites
{
    public class CreateConviteDto
    {
        [Required(ErrorMessage = "É necessário informar um email para enviar o convite.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        [MaxLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
        public string EmailDestino { get; set; } = string.Empty;
    }
}