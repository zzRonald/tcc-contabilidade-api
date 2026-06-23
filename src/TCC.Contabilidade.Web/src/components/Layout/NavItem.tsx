import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { type LucideIcon } from 'lucide-react';
import { cn } from '../../utils/cn';

interface NavItemProps {
  label: string;
  path: string;
  icon: LucideIcon;
  onClick?: () => void;
}

export const NavItem: React.FC<NavItemProps> = ({ label, path, icon: Icon, onClick }) => {
  const location = useLocation();
  const isActive = location.pathname === path || (path !== '/' && location.pathname.startsWith(path));

  return (
    <Link
      to={path}
      onClick={onClick}
      className={cn(
        'group flex items-center px-3 py-2 text-sm font-medium rounded-md transition-all duration-200',
        isActive
          ? 'bg-blue-50 text-blue-700'
          : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
      )}
    >
      <Icon
        className={cn(
          'mr-3 h-5 w-5 flex-shrink-0 transition-colors duration-200',
          isActive ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-500'
        )}
      />
      {label}
    </Link>
  );
};
