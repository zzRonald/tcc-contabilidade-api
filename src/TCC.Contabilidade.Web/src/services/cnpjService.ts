import apiClient from '../api/axios';
import { type ApiResponse } from '../interfaces/api';
import { type CnpjResponse } from '../interfaces/cnpj';

export const cnpjService = {
  async consultar(cnpj: string) {
    const response = await apiClient.get<ApiResponse<CnpjResponse>>(`/Cnpj/${cnpj}`);
    return response.data;
  },
};
