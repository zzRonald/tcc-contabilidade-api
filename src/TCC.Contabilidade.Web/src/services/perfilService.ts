import apiClient from '../api/axios';
import type { ApiResponse } from '../interfaces/api';
import type { User } from '../interfaces/auth';
import type { UpdateProfileRequest, ChangePasswordRequest } from '../interfaces/perfil';

export const perfilService = {
  getPerfil: async () => {
    const response = await apiClient.get<ApiResponse<User>>('/Perfil');
    return response.data;
  },

  updatePerfil: async (data: UpdateProfileRequest) => {
    const response = await apiClient.put<ApiResponse<null>>('/Perfil', data);
    return response.data;
  },

  alterarSenha: async (data: ChangePasswordRequest) => {
    const response = await apiClient.post<ApiResponse<null>>('/Perfil/alterar-senha', data);
    return response.data;
  },
};
