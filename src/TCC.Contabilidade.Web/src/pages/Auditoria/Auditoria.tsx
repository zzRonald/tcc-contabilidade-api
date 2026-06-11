import React, { useState, useEffect, useCallback } from 'react';
import { auditService } from '../../services/auditService';
import type { AuditLog, AuditLogFilter } from '../../interfaces/audit';
import type { PaginationMetadata } from '../../interfaces/api';
import { ClipboardList, Filter, Calendar, User, ShieldCheck, ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '../../components/Button';
import { cn } from '../../utils/cn';

export const Auditoria: React.FC = () => {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pagination, setPagination] = useState<PaginationMetadata | null>(null);
  const [filters, setFilters] = useState<AuditLogFilter>({
    pagina: 1,
    tamanhoPagina: 15,
    dataInicio: '',
    dataFim: '',
    acao: ''
  });

  const fetchLogs = useCallback(async (currentFilters: AuditLogFilter) => {
    try {
      setLoading(true);
      setError(null);

      // Limpar filtros vazios antes de enviar
      const params = { ...currentFilters };
      if (!params.dataInicio) delete params.dataInicio;
      if (!params.dataFim) delete params.dataFim;
      if (!params.acao) delete params.acao;

      const response = await auditService.getLogs(params);

      if (response.sucesso) {
        setLogs(response.dados);
        setPagination(response.paginacao || null);
      } else {
        setError(response.mensagem);
      }
    } catch (err) {
      setError('Falha ao carregar logs de auditoria. Por favor, tente novamente.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchLogs(filters);
  }, [fetchLogs, filters.pagina]); // Recarrega quando a página muda

  const handleFilterSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (filters.pagina === 1) {
      fetchLogs(filters);
    } else {
      setFilters(prev => ({ ...prev, pagina: 1 }));
    }
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, pagina: newPage }));
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <ClipboardList className="h-8 w-8 text-blue-600" />
            Auditoria do Sistema
          </h1>
          <p className="text-sm text-gray-500">Acompanhe as ações realizadas na plataforma.</p>
        </div>
      </div>

      {/* Filtros */}
      <div className="bg-white p-4 shadow rounded-lg">
        <form onSubmit={handleFilterSubmit} className="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
          <div>
            <label className="block text-xs font-medium text-gray-700 uppercase mb-1">Ação</label>
            <input
              type="text"
              placeholder="Ex: Login, Create..."
              className="block w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-blue-500 focus:border-blue-500"
              value={filters.acao}
              onChange={(e) => setFilters(prev => ({ ...prev, acao: e.target.value }))}
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-700 uppercase mb-1">Data Início</label>
            <input
              type="date"
              className="block w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-blue-500 focus:border-blue-500"
              value={filters.dataInicio}
              onChange={(e) => setFilters(prev => ({ ...prev, dataInicio: e.target.value }))}
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-700 uppercase mb-1">Data Fim</label>
            <input
              type="date"
              className="block w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:ring-blue-500 focus:border-blue-500"
              value={filters.dataFim}
              onChange={(e) => setFilters(prev => ({ ...prev, dataFim: e.target.value }))}
            />
          </div>
          <div className="flex gap-2">
            <Button type="submit" className="flex-1">
              <Filter className="h-4 w-4 mr-2" />
              Filtrar
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                const reset = { pagina: 1, tamanhoPagina: 15, dataInicio: '', dataFim: '', acao: '' };
                setFilters(reset);
                fetchLogs(reset);
              }}
            >
              Limpar
            </Button>
          </div>
        </form>
      </div>

      {/* Conteúdo */}
      <div className="bg-white shadow rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-12 text-center">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-600 border-t-transparent"></div>
            <p className="mt-2 text-gray-500">Carregando logs...</p>
          </div>
        ) : error ? (
          <div className="p-12 text-center text-red-500">
            <p>{error}</p>
            <Button variant="outline" className="mt-4" onClick={() => fetchLogs(filters)}>
              Tentar novamente
            </Button>
          </div>
        ) : logs.length === 0 ? (
          <div className="p-12 text-center">
            <ClipboardList className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhum log encontrado</h3>
            <p className="mt-1 text-sm text-gray-500">Tente ajustar seus filtros.</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Data / Hora</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Usuário</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Ação</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Entidade</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">IP</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {logs.map((log) => (
                  <tr key={log.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      <div className="flex items-center gap-2">
                        <Calendar className="h-4 w-4 text-gray-400" />
                        {new Date(log.dataHora).toLocaleString('pt-BR')}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      <div className="flex items-center gap-2">
                        <User className="h-4 w-4 text-gray-400" />
                        {log.usuarioNome || 'Sistema'}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600">
                      {log.acao}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      <div className="flex items-center gap-2">
                        <ShieldCheck className="h-4 w-4 text-gray-400" />
                        {log.entidade} {log.entidadeId && <span className="text-xs bg-gray-100 px-1 rounded">#{log.entidadeId.substring(0, 8)}</span>}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 font-mono">
                      {log.ip}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Paginação */}
        {pagination && pagination.totalPaginas > 1 && (
          <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
            <div className="flex-1 flex justify-between sm:hidden">
              <Button
                variant="outline"
                size="sm"
                disabled={pagination.paginaAtual === 1}
                onClick={() => handlePageChange(pagination.paginaAtual - 1)}
              >
                Anterior
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={pagination.paginaAtual === pagination.totalPaginas}
                onClick={() => handlePageChange(pagination.paginaAtual + 1)}
              >
                Próxima
              </Button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  Mostrando <span className="font-medium">{(pagination.paginaAtual - 1) * pagination.tamanhoPagina + 1}</span> até{' '}
                  <span className="font-medium">
                    {Math.min(pagination.paginaAtual * pagination.tamanhoPagina, pagination.totalRegistros)}
                  </span>{' '}
                  de <span className="font-medium">{pagination.totalRegistros}</span> resultados
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                  <button
                    disabled={pagination.paginaAtual === 1}
                    onClick={() => handlePageChange(pagination.paginaAtual - 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    <ChevronLeft className="h-5 w-5" />
                  </button>
                  {/* Simplificado: mostra até 5 páginas */}
                  {[...Array(Math.min(5, pagination.totalPaginas))].map((_, i) => {
                    const pageNum = pagination.paginaAtual > 3
                      ? pagination.paginaAtual - 2 + i
                      : i + 1;

                    if (pageNum > pagination.totalPaginas) return null;

                    return (
                      <button
                        key={pageNum}
                        onClick={() => handlePageChange(pageNum)}
                        className={cn(
                          'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                          pagination.paginaAtual === pageNum
                            ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                            : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                        )}
                      >
                        {pageNum}
                      </button>
                    );
                  })}
                  <button
                    disabled={pagination.paginaAtual === pagination.totalPaginas}
                    onClick={() => handlePageChange(pagination.paginaAtual + 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    <ChevronRight className="h-5 w-5" />
                  </button>
                </nav>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
