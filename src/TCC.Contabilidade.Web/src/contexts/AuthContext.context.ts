import { createContext } from 'react';
import { type AuthContextData } from './AuthContext.types';

export const AuthContext = createContext<AuthContextData>({} as AuthContextData);
