import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar o token JWT em cada requisição
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('@TCC:token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros de resposta de forma global
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      // Opcional: Redirecionar para login ou limpar storage
      localStorage.removeItem('@TCC:token');
      localStorage.removeItem('@TCC:user');
      // window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
