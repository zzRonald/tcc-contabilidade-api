import apiClient from '../api/axios';
import type { ApiResponse } from '../interfaces/api';
import type { Convite, CriarConviteRequest, CriarConviteResponse } from '../interfaces/convite';

export const conviteService = {
  async listarConvites(page = 1, pageSize = 10): Promise<ApiResponse<Convite[]>> {
    const response = await apiClient.get<ApiResponse<Convite[]>>(`/convites?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },

  async criarConvite(request: CriarConviteRequest): Promise<CriarConviteResponse> {
    const response = await apiClient.post<CriarConviteResponse>('/convites', request);
    return response.data;
  }
};
