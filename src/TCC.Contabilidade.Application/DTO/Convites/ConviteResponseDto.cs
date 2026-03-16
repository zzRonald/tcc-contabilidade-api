using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Contabilidade.Application.DTOs.Convites
{
    public class ConviteResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string EmailDestino { get; set; } = string.Empty;

        public DateTime DataExpiracao { get; set; }

        public bool Utilizado { get; set; }
    }
}
