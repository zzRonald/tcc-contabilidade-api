import { useEffect, useState, useCallback } from 'react';
import { conviteService } from '../../services/conviteService';
import { type Convite } from '../../interfaces/convite';
import { type PaginationMetadata } from '../../interfaces/api';
import { Button } from '../../components/Button';
import { Plus, Mail, ChevronLeft, ChevronRight, Search, Clock, CheckCircle, XCircle } from 'lucide-react';
import { cn } from '../../utils/cn';
import { ConviteModal } from './ConviteModal';

export const ConviteList = () => {
  const [convites, setConvites] = useState<Convite[]>([]);
  const [pagination, setPagination] = useState<PaginationMetadata | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);

  const fetchConvites = useCallback(async (page = 1, showLoading = false) => {
    try {
      if (showLoading) setLoading(true);
      const response = await conviteService.listarConvites(page, 10);
      if (response.sucesso) {
        setConvites(response.dados);
        setPagination(response.paginacao || null);
      } else {
        setError(response.mensagem);
      }
    } catch (err) {
      setError('Erro ao carregar convites. Tente novamente mais tarde.');
      console.error(err);
    } finally {
      if (showLoading) setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchConvites(1, true);
  }, [fetchConvites]);

  const filteredConvites = convites.filter(convite =>
    convite.emailDestino.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getStatusInfo = (convite: Convite) => {
    if (convite.utilizado) {
      return {
        label: 'Aceito',
        color: 'text-green-700 bg-green-50',
        icon: <CheckCircle className="h-4 w-4" />
      };
    }

    const dataExpiracao = new Date(convite.dataExpiracao);
    if (dataExpiracao < new Date()) {
      return {
        label: 'Expirado',
        color: 'text-red-700 bg-red-50',
        icon: <XCircle className="h-4 w-4" />
      };
    }

    return {
      label: 'Pendente',
      color: 'text-yellow-700 bg-yellow-50',
      icon: <Clock className="h-4 w-4" />
    };
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Convites Enviados</h1>
          <p className="text-sm text-gray-500">Acompanhe o status dos convites enviados aos seus clientes.</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)} className="flex items-center space-x-2">
          <Plus className="h-4 w-4" />
          <span>Novo Convite</span>
        </Button>
      </div>

      <div className="bg-white shadow rounded-lg overflow-hidden">
        <div className="p-4 border-b border-gray-200">
          <div className="relative max-w-sm">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Buscar por e-mail..."
              className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {loading ? (
          <div className="p-12 text-center">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-600 border-t-transparent"></div>
            <p className="mt-2 text-gray-500">Carregando convites...</p>
          </div>
        ) : error ? (
          <div className="p-12 text-center">
            <p className="text-red-500">{error}</p>
            <Button variant="outline" className="mt-4" onClick={() => fetchConvites()}>
              Tentar novamente
            </Button>
          </div>
        ) : filteredConvites.length === 0 ? (
          <div className="p-12 text-center">
            <Mail className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhum convite encontrado</h3>
            <p className="mt-1 text-sm text-gray-500">Comece enviando um novo convite para um cliente.</p>
            <div className="mt-6">
              <Button variant="outline" onClick={() => setIsModalOpen(true)}>
                <Plus className="mr-2 h-4 w-4" />
                Novo Convite
              </Button>
            </div>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    E-mail do Cliente
                  </th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Expira em
                  </th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Token
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredConvites.map((convite) => {
                  const status = getStatusInfo(convite);
                  return (
                    <tr key={convite.token} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="flex-shrink-0 h-8 w-8 bg-gray-100 rounded-full flex items-center justify-center">
                            <Mail className="h-4 w-4 text-gray-600" />
                          </div>
                          <div className="ml-4">
                            <div className="text-sm font-medium text-gray-900">{convite.emailDestino}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-500">
                          {new Date(convite.dataExpiracao).toLocaleDateString()} {new Date(convite.dataExpiracao).toLocaleTimeString()}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={cn(
                          "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium space-x-1",
                          status.color
                        )}>
                          {status.icon}
                          <span>{status.label}</span>
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <code className="text-xs bg-gray-100 px-2 py-1 rounded text-gray-600">
                          {convite.token.substring(0, 8)}...
                        </code>
                      </td>
                    </tr>
                  );
                })}
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
                onClick={() => fetchConvites(pagination.paginaAtual - 1)}
              >
                Anterior
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={pagination.paginaAtual === pagination.totalPaginas}
                onClick={() => fetchConvites(pagination.paginaAtual + 1)}
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
                    onClick={() => fetchConvites(pagination.paginaAtual - 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    <span className="sr-only">Anterior</span>
                    <ChevronLeft className="h-5 w-5" />
                  </button>
                  {[...Array(pagination.totalPaginas)].map((_, i) => (
                    <button
                      key={i + 1}
                      onClick={() => fetchConvites(i + 1)}
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
                    onClick={() => fetchConvites(pagination.paginaAtual + 1)}
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

      <ConviteModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={() => {
          setIsModalOpen(false);
          fetchConvites(1, true);
        }}
      />
    </div>
  );
};
