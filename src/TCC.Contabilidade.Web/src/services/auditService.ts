import apiClient from '../api/axios';
import type { ApiResponse } from '../interfaces/api';
import type { AuditLog, AuditLogFilter } from '../interfaces/audit';

export const auditService = {
  getLogs: async (filtros: AuditLogFilter) => {
    const response = await apiClient.get<ApiResponse<AuditLog[]>>('/Auditoria', {
      params: filtros,
    });
    return response.data;
  },
};
