export interface Empresa {
  id: string;
  nomeFantasia: string;
  cnpj: string;
}

export interface CreateEmpresaRequest {
  nome: string;
  cnpj: string;
}

export interface UpdateEmpresaRequest {
  nome: string;
  cnpj: string;
}
