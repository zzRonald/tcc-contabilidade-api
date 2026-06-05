import apiClient from '../api/axios';
import { type ApiResponse } from '../interfaces/api';
import {
  type Empresa,
  type CreateEmpresaRequest,
  type UpdateEmpresaRequest,
} from '../interfaces/empresa';

export const empresaService = {
  async getAll(page = 1, pageSize = 10) {
    const response = await apiClient.get<ApiResponse<Empresa[]>>('/Empresas', {
      params: { page, pageSize },
    });
    return response.data;
  },

  async getById(id: string) {
    const response = await apiClient.get<ApiResponse<Empresa>>(`/Empresas/${id}`);
    return response.data;
  },

  async create(data: CreateEmpresaRequest) {
    const response = await apiClient.post<ApiResponse<null>>('/Empresas', data);
    return response.data;
  },

  async update(id: string, data: UpdateEmpresaRequest) {
    const response = await apiClient.put<ApiResponse<null>>(`/Empresas/${id}`, data);
    return response.data;
  },

  async delete(id: string) {
    const response = await apiClient.delete<ApiResponse<null>>(`/Empresas/${id}`);
    return response.data;
  },
};
