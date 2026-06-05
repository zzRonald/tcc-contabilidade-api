import { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { empresaService } from '../../services/empresaService';
import { type Empresa } from '../../interfaces/empresa';
import { type PaginationMetadata } from '../../interfaces/api';
import { Button } from '../../components/Button';
import { Plus, Edit, Trash2, ChevronLeft, ChevronRight, Building2, Search } from 'lucide-react';

export const EmpresaList = () => {
  const [empresas, setEmpresas] = useState<Empresa[]>([]);
  const [pagination, setPagination] = useState<PaginationMetadata | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');

  const fetchEmpresas = useCallback(async (page = 1, showLoading = false) => {
    try {
      if (showLoading) setLoading(true);
      const response = await empresaService.getAll(page, 10);
      if (response.sucesso) {
        setEmpresas(response.dados);
        setPagination(response.paginacao || null);
      } else {
        setError(response.mensagem);
      }
    } catch (err) {
      setError('Erro ao carregar empresas. Tente novamente mais tarde.');
      console.error(err);
    } finally {
      if (showLoading) setLoading(false);
    }
  }, []);

  useEffect(() => {
    const load = async () => {
      await fetchEmpresas(1, true);
    };
    load();
  }, [fetchEmpresas]);

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja excluir esta empresa?')) return;

    try {
      const response = await empresaService.delete(id);
      if (response.sucesso) {
        fetchEmpresas(pagination?.paginaAtual);
      } else {
        alert(response.mensagem);
      }
    } catch (err) {
      alert('Erro ao excluir empresa.');
      console.error(err);
    }
  };

  const filteredEmpresas = empresas.filter(empresa =>
    empresa.nomeFantasia.toLowerCase().includes(searchTerm.toLowerCase()) ||
    empresa.cnpj.includes(searchTerm)
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Gestão de Empresas</h1>
          <p className="text-sm text-gray-500">Gerencie as empresas vinculadas ao seu contexto.</p>
        </div>
        <Link to="/empresas/nova">
          <Button className="flex items-center space-x-2">
            <Plus className="h-4 w-4" />
            <span>Nova Empresa</span>
          </Button>
        </Link>
      </div>

      <div className="bg-white shadow rounded-lg overflow-hidden">
        <div className="p-4 border-b border-gray-200">
          <div className="relative max-w-sm">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Buscar por nome ou CNPJ..."
              className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {loading ? (
          <div className="p-12 text-center">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-600 border-t-transparent"></div>
            <p className="mt-2 text-gray-500">Carregando empresas...</p>
          </div>
        ) : error ? (
          <div className="p-12 text-center">
            <p className="text-red-500">{error}</p>
            <Button variant="outline" className="mt-4" onClick={() => fetchEmpresas()}>
              Tentar novamente
            </Button>
          </div>
        ) : filteredEmpresas.length === 0 ? (
          <div className="p-12 text-center">
            <Building2 className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhuma empresa encontrada</h3>
            <p className="mt-1 text-sm text-gray-500">Comece cadastrando uma nova empresa.</p>
            <div className="mt-6">
              <Link to="/empresas/nova">
                <Button variant="outline">
                  <Plus className="mr-2 h-4 w-4" />
                  Nova Empresa
                </Button>
              </Link>
            </div>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Empresa
                  </th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    CNPJ
                  </th>
                  <th scope="col" className="relative px-6 py-3">
                    <span className="sr-only">Ações</span>
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredEmpresas.map((empresa) => (
                  <tr key={empresa.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="flex-shrink-0 h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                          <Building2 className="h-6 w-6 text-blue-600" />
                        </div>
                        <div className="ml-4">
                          <div className="text-sm font-medium text-gray-900">{empresa.nomeFantasia}</div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-500">{empresa.cnpj}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex justify-end space-x-2">
                        <Link to={`/empresas/editar/${empresa.id}`}>
                          <Button variant="outline" size="sm" title="Editar">
                            <Edit className="h-4 w-4" />
                          </Button>
                        </Link>
                        <Button
                          variant="danger"
                          size="sm"
                          title="Excluir"
                          onClick={() => handleDelete(empresa.id)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {pagination && pagination.totalPaginas > 1 && (
          <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
            <div className="flex-1 flex justify-between sm:hidden">
              <Button
                variant="outline"
                size="sm"
                disabled={pagination.paginaAtual === 1}
                onClick={() => fetchEmpresas(pagination.paginaAtual - 1)}
              >
                Anterior
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={pagination.paginaAtual === pagination.totalPaginas}
                onClick={() => fetchEmpresas(pagination.paginaAtual + 1)}
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
                    onClick={() => fetchEmpresas(pagination.paginaAtual - 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    <span className="sr-only">Anterior</span>
                    <ChevronLeft className="h-5 w-5" />
                  </button>
                  {[...Array(pagination.totalPaginas)].map((_, i) => (
                    <button
                      key={i + 1}
                      onClick={() => fetchEmpresas(i + 1)}
                      className={cn(
                        'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                        pagination.paginaAtual === i + 1
                          ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                          : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                      )}
                    >
                      {i + 1}
                    </button>
                  ))}
                  <button
                    disabled={pagination.paginaAtual === pagination.totalPaginas}
                    onClick={() => fetchEmpresas(pagination.paginaAtual + 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    <span className="sr-only">Próxima</span>
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

// Aux function for conditional classes
function cn(...classes: (string | boolean | undefined)[]) {
  return classes.filter(Boolean).join(' ');
}
