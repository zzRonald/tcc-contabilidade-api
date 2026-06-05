export interface ApiResponse<T> {
  status: boolean;
  message: string;
  data: T;
  errors: string[];
}

export interface PaginationMetadata {
  paginaAtual: number;
  tamanhoPagina: number;
  totalRegistros: number;
  totalPaginas: number;
}

export interface PagedApiResponse<T> extends ApiResponse<T> {
  metadata?: PaginationMetadata;
}
