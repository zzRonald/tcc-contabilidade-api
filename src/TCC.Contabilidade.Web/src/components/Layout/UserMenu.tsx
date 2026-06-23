import React, { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { LogOut, User as UserIcon, ChevronDown } from 'lucide-react';
import { cn } from '../../utils/cn';

export const UserMenu: React.FC = () => {
  const { user, signOut } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <div className="relative" ref={menuRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-700">
          <UserIcon className="h-5 w-5" />
        </div>
        <div className="hidden md:flex flex-col text-left">
          <span className="text-sm font-semibold text-gray-700 leading-none">{user?.nome}</span>
          <span className="text-[10px] text-gray-500 uppercase font-bold mt-1">
            {user?.tipoUsuario}
          </span>
        </div>
        <ChevronDown className={cn("h-4 w-4 text-gray-400 transition-transform", isOpen && "rotate-180")} />
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-48 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 z-50">
          <div className="py-1" role="menu">
            <Link
              to="/perfil"
              onClick={() => setIsOpen(false)}
              className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
              role="menuitem"
            >
              <UserIcon className="mr-3 h-4 w-4 text-gray-400" />
              Meu Perfil
            </Link>
            <button
              onClick={() => {
                setIsOpen(false);
                signOut();
              }}
              className="flex w-full items-center px-4 py-2 text-sm text-red-700 hover:bg-red-50"
              role="menuitem"
            >
              <LogOut className="mr-3 h-4 w-4 text-red-400" />
              Sair
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
