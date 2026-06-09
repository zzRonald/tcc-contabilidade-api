export interface Convite {
  token: string;
  emailDestino: string;
  dataExpiracao: string;
  utilizado: boolean;
}

export interface CriarConviteRequest {
  emailCliente: string;
}

export interface CriarConviteResponse {
  mensagem: string;
  token: string;
}
