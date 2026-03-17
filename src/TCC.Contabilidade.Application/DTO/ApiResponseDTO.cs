using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Contabilidade.Application.DTOs
{
    public class ApiResponseDTO<T>
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public T? Dados { get; set; }
        public string? Erro { get; set; }
        public int? Codigo { get; set; }

        public static ApiResponseDTO<T> Success(T data, string mensagem = "Operação realizada com sucesso")
        {
            return new ApiResponseDTO<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = data
            };
        }

        public static ApiResponseDTO<T> Fail(string erro, int codigo)
        {
            return new ApiResponseDTO<T>
            {
                Sucesso = false,
                Erro = erro,
                Codigo = codigo
            };
        }
    }
}