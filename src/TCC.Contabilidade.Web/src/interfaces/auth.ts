export interface User {
  id: string;
  nome: string;
  email: string;
  tipoUsuario: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiraEm: number;
  usuario: User;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegisterWithInviteRequest {
  invitationToken: string;
  nome: string;
  email: string;
  senha: string;
}
