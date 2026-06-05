export interface ApiResponse<T> {
  sucesso: boolean;
  mensagem: string;
  dados: T;
  paginacao?: PaginationMetadata;
  erro?: unknown;
  codigo?: number;
}

export interface PaginationMetadata {
  paginaAtual: number;
  tamanhoPagina: number;
  totalRegistros: number;
  totalPaginas: number;
}
