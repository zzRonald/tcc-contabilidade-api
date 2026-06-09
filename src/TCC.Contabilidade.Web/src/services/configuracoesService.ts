import apiClient from '../api/axios';
import type { ApiResponse } from '../interfaces/api';
import type { CompanyConfig } from '../interfaces/configuracoes';

export const configuracoesService = {
  getConfiguracoes: async (companyId: string) => {
    const response = await apiClient.get<ApiResponse<CompanyConfig>>('/Configuracoes', {
      headers: {
        'X-Company-Id': companyId,
      },
    });
    return response.data;
  },

  updateConfiguracoes: async (companyId: string, data: CompanyConfig) => {
    const response = await apiClient.put<ApiResponse<null>>('/Configuracoes', data, {
      headers: {
        'X-Company-Id': companyId,
      },
    });
    return response.data;
  },
};
