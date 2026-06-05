import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { Button } from '../Button';
import { LogOut, User as UserIcon, Building2, Home as HomeIcon } from 'lucide-react';

export const Layout: React.FC = () => {
  const { user, signOut } = useAuth();
  const location = useLocation();

  const navItems = [
    { label: 'Início', path: '/', icon: HomeIcon },
    { label: 'Empresas', path: '/empresas', icon: Building2, roles: ['Contador', 'Admin'] },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm sticky top-0 z-50">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex h-16 justify-between items-center">
            <div className="flex items-center space-x-8">
              <Link to="/" className="flex items-center">
                <span className="text-xl font-bold text-blue-600">TCC Contabilidade</span>
              </Link>

              <div className="hidden md:flex items-center space-x-4">
                {navItems.map((item) => {
                  if (item.roles && !item.roles.includes(user?.tipoUsuario || '')) return null;

                  const Icon = item.icon;
                  const isActive = location.pathname === item.path || (item.path !== '/' && location.pathname.startsWith(item.path));

                  return (
                    <Link
                      key={item.path}
                      to={item.path}
                      className={`flex items-center space-x-1 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                        isActive
                          ? 'text-blue-600 bg-blue-50'
                          : 'text-gray-600 hover:text-blue-600 hover:bg-gray-50'
                      }`}
                    >
                      <Icon className="h-4 w-4" />
                      <span>{item.label}</span>
                    </Link>
                  );
                })}
              </div>
            </div>

            <div className="flex items-center space-x-4">
              <div className="hidden sm:flex items-center space-x-2 text-gray-700">
                <UserIcon className="h-5 w-5" />
                <div className="flex flex-col">
                  <span className="text-sm font-medium leading-none">{user?.nome}</span>
                  <span className="text-[10px] text-gray-500 uppercase font-bold mt-1">
                    {user?.tipoUsuario}
                  </span>
                </div>
              </div>
              <Button variant="outline" size="sm" onClick={signOut} className="flex items-center space-x-1">
                <LogOut className="h-4 w-4" />
                <span className="hidden sm:inline">Sair</span>
              </Button>
            </div>
          </div>
        </div>
      </nav>

      <main className="mx-auto max-w-7xl py-8 px-4 sm:px-6 lg:px-8">
        <Outlet />
      </main>
    </div>
  );
};
