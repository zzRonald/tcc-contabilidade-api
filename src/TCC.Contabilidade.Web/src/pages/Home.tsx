import { useAuth } from '../hooks/useAuth';
import { Button } from '../components/Button';
import { LogOut, User as UserIcon } from 'lucide-react';

export const Home = () => {
  const { user, signOut } = useAuth();

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex h-16 justify-between items-center">
            <div className="flex items-center">
              <span className="text-xl font-bold text-blue-600">TCC Contabilidade</span>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2 text-gray-700">
                <UserIcon className="h-5 w-5" />
                <span className="text-sm font-medium">{user?.nome}</span>
                <span className="rounded-full bg-blue-100 px-2 py-0.5 text-xs font-semibold text-blue-800 uppercase">
                  {user?.tipoUsuario}
                </span>
              </div>
              <Button variant="outline" size="sm" onClick={signOut} className="flex items-center space-x-1">
                <LogOut className="h-4 w-4" />
                <span>Sair</span>
              </Button>
            </div>
          </div>
        </div>
      </nav>

      <main className="mx-auto max-w-7xl py-12 px-4 sm:px-6 lg:px-8">
        <div className="rounded-lg border-2 border-dashed border-gray-200 bg-white p-12 text-center">
          <h1 className="text-4xl font-extrabold tracking-tight text-gray-900 sm:text-5xl">
            Bem-vindo ao Sistema
          </h1>
          <p className="mt-6 text-lg text-gray-500">
            Você está autenticado como <span className="font-semibold text-gray-900">{user?.email}</span>.
          </p>
          <div className="mt-10 flex justify-center gap-x-6">
            <p className="text-sm text-gray-400 italic">
              Esta é uma área protegida. Somente usuários autenticados podem ver este conteúdo.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
};
