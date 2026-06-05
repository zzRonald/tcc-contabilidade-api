import { Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { Building2, ArrowRight } from 'lucide-react';

export const Home = () => {
  const { user } = useAuth();

  const isContadorOrAdmin = user?.tipoUsuario === 'Contador' || user?.tipoUsuario === 'Admin';

  return (
    <div className="space-y-8">
      <div className="bg-white rounded-lg shadow-sm p-8 border border-gray-100">
        <h1 className="text-3xl font-extrabold tracking-tight text-gray-900 sm:text-4xl">
          Olá, {user?.nome}!
        </h1>
        <p className="mt-4 text-lg text-gray-500">
          Bem-vindo ao painel do <span className="font-semibold text-blue-600">TCC Contabilidade</span>.
          Aqui você pode gerenciar suas empresas e processos contábeis.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {isContadorOrAdmin && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow">
            <div className="p-6">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
                <Building2 className="h-6 w-6 text-blue-600" />
              </div>
              <h3 className="text-xl font-bold text-gray-900">Gestão de Empresas</h3>
              <p className="mt-2 text-gray-500">
                Cadastre novas empresas, consulte CNPJs e gerencie sua carteira de clientes.
              </p>
              <Link
                to="/empresas"
                className="mt-6 inline-flex items-center text-blue-600 font-semibold hover:text-blue-700"
              >
                Acessar agora
                <ArrowRight className="ml-2 h-4 w-4" />
              </Link>
            </div>
          </div>
        )}

        {/* Outros cards de funcionalidades futuras podem ser adicionados aqui */}
      </div>
    </div>
  );
};
