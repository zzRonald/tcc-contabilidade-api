using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Contabilidade.Application.DTOs;

public class ApiResponseDTO<T>
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public T? Dados { get; set; }
    public object? Erro { get; set; }
    public int? Codigo { get; set; }

    public static ApiResponseDTO<T> Success(T dados, string mensagem = "Operação realizada com sucesso")
    {
        return new ApiResponseDTO<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados
        };
    }

    public static ApiResponseDTO<T> Fail(string mensagem, int codigo = 400, object? erro = null)
    {
        return new ApiResponseDTO<T>
        {
            Sucesso = false,
            Mensagem = mensagem,
            Codigo = codigo,
            Erro = erro
        };
    }
}