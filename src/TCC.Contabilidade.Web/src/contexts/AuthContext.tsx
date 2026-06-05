import React, { useState, useEffect, useCallback } from 'react';
import apiClient from '../api/axios';
import { type AuthResponse, type LoginRequest, type RegisterWithInviteRequest, type User } from '../interfaces/auth';
import { type ApiResponse } from '../interfaces/api';
import { AuthContext } from './AuthContext.context';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    function loadStorageData() {
      try {
        const storagedUser = localStorage.getItem('@TCC:user');
        const storagedToken = localStorage.getItem('@TCC:token');

        if (storagedUser && storagedToken) {
          setUser(JSON.parse(storagedUser));
        }
      } catch (error) {
        console.error('Erro ao carregar dados do localStorage:', error);
        localStorage.removeItem('@TCC:token');
        localStorage.removeItem('@TCC:user');
      } finally {
        setLoading(false);
      }
    }

    loadStorageData();
  }, []);

  const signIn = useCallback(async ({ email, senha }: LoginRequest) => {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/Auth/login', {
      email,
      senha,
    });

    const { accessToken, usuario } = response.data.dados;

    setUser(usuario);

    localStorage.setItem('@TCC:user', JSON.stringify(usuario));
    localStorage.setItem('@TCC:token', accessToken);
  }, []);

  const signUpWithInvite = useCallback(async (data: RegisterWithInviteRequest) => {
    await apiClient.post<ApiResponse<unknown>>('/Auth/register-with-invite', {
      invitationToken: data.invitationToken,
      nome: data.nome,
      email: data.email,
      senha: data.senha,
    });
  }, []);

  const signOut = useCallback(() => {
    localStorage.removeItem('@TCC:token');
    localStorage.removeItem('@TCC:user');
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider
      value={{
        authenticated: !!user,
        user,
        loading,
        signIn,
        signOut,
        signUpWithInvite,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
