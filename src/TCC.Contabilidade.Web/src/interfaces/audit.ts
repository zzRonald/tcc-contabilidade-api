export interface AuditLog {
  id: number;
  usuarioId: string | null;
  usuarioNome: string | null;
  acao: string;
  entidade: string;
  entidadeId: string | null;
  dataHora: string;
  ip: string;
}

export interface AuditLogFilter {
  dataInicio?: string;
  dataFim?: string;
  usuarioId?: string;
  acao?: string;
  pagina?: number;
  tamanhoPagina?: number;
}
