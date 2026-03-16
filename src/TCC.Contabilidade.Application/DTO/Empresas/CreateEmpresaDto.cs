using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Contabilidade.Application.DTO.Empresas;

public class CreateEmpresaDto
{
    public string Nome { get; set; } = string.Empty;

    public string CNPJ { get; set; } = string.Empty;
}