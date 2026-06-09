export interface UpdateProfileRequest {
  nome: string;
  email: string;
}

export interface ChangePasswordRequest {
  senhaAtual: string;
  novaSenha: string;
}
