import React from 'react';
import { Link } from 'react-router-dom';
import { NavItem } from './NavItem';
import { Home, Building2, Mail, ClipboardList, Settings, X } from 'lucide-react';
import { useAuth } from '../../hooks/useAuth';
import { cn } from '../../utils/cn';

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
  const { user } = useAuth();

  const navItems = [
    { label: 'Início', path: '/', icon: Home },
    { label: 'Empresas', path: '/empresas', icon: Building2, roles: ['Contador', 'Admin'] },
    { label: 'Convites', path: '/convites', icon: Mail, roles: ['Contador'] },
    { label: 'Auditoria', path: '/auditoria', icon: ClipboardList, roles: ['Contador', 'Admin'] },
    { label: 'Configurações', path: '/configuracoes', icon: Settings, roles: ['Contador', 'Admin'] },
  ];

  const filteredItems = navItems.filter(
    (item) => !item.roles || item.roles.includes(user?.tipoUsuario || '')
  );

  return (
    <>
      {/* Mobile Overlay */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-gray-600 bg-opacity-75 transition-opacity lg:hidden"
          onClick={onClose}
        />
      )}

      {/* Sidebar Sidebar */}
      <div
        className={cn(
          'fixed inset-y-0 left-0 z-50 w-64 transform bg-white shadow-xl transition-transform duration-300 ease-in-out lg:static lg:translate-x-0',
          isOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="flex h-16 items-center justify-between px-6 lg:justify-center">
          <Link to="/" className="flex items-center" onClick={onClose}>
            <span className="text-xl font-bold text-blue-600">TCC Contabilidade</span>
          </Link>
          <button onClick={onClose} className="rounded-md p-2 text-gray-500 hover:bg-gray-100 lg:hidden">
            <X className="h-6 w-6" />
          </button>
        </div>

        <nav className="mt-5 space-y-1 px-4">
          {filteredItems.map((item) => (
            <NavItem
              key={item.path}
              label={item.label}
              path={item.path}
              icon={item.icon}
              onClick={onClose}
            />
          ))}
        </nav>
      </div>
    </>
  );
};
