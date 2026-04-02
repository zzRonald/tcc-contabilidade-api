# Endpoints da API — TCC Contabilidade .  API

> **Fonte da verdade:** Controllers da aplicação + Swagger
>
> **Status**
> - **[IMPLEMENTADO]** = existe no código
> - **[PLANEJADO]** = backlog/documentação futura

---

## Convenções

### Base
- Todas as rotas são prefixadas com `/api`
- Exemplo:
  - `/api/auth/login`
  - `/api/empresas`

---

### Auth (JWT)

Header:

~~~
Authorization: Bearer {token}
~~~

---

### RBAC (Roles)

- Admin
- Contador
- Usuario

---

### Envelope de resposta

~~~json
{
  "status": true,
  "message": "string",
  "data": {},
  "errors": []
}
~~~

---

### Status HTTP

| Código | Descrição |
|------|----------|
| 200 | Sucesso |
| 201 | Criado |
| 400 | Erro de validação |
| 401 | Não autenticado |
| 403 | Sem permissão |
| 404 | Não encontrado |
| 429 | Rate limit |
| 500 | Erro interno |

---

# [IMPLEMENTADO] Endpoints atuais

---

## 🔐 Auth

| Método | Rota | Auth | Controller | Request | Response | Obs. |
|------|------|------|------------|--------|---------|------|
| POST | `/api/auth/login` | Pública | AuthController.Login | LoginRequest | ApiResponse<TokenResponse> | Gera JWT |
| POST | `/api/auth/register-with-invite` | Pública | AuthController.RegisterWithInvite | RegisterRequest | ApiResponse<UserResponse> | Cadastro via convite |

---

## 📩 Convites

| Método | Rota | Auth | Controller | Request | Response | Obs. |
|------|------|------|------------|--------|---------|------|
| POST | `/api/invites` | Contador | InviteController.Create | CreateInviteRequest | ApiResponse<InviteResponse> | Cria convite |
| GET | `/api/invites` | Contador | InviteController.Get | — | ApiResponse<List<InviteResponse>> | Lista convites |

---

## 🏢 Empresas

| Método | Rota | Auth | Controller | Request | Response | Obs. |
|------|------|------|------------|--------|---------|------|
| POST | `/api/empresas` | Contador | EmpresasController.Create | CreateEmpresaDto | ApiResponse<EmpresaResponse> | Cria empresa |
| GET | `/api/empresas` | Contador | EmpresasController.Get | — | ApiResponse<List<EmpresaResponse>> | Lista empresas |
| GET | `/api/empresas/{id}` | Contador | EmpresasController.GetById | — | ApiResponse<EmpresaResponse> | Busca por ID |
| PUT | `/api/empresas/{id}` | Contador | EmpresasController.Update | UpdateEmpresaDto | ApiResponse<EmpresaResponse> | Atualiza empresa |
| DELETE | `/api/empresas/{id}` | Contador | EmpresasController.Delete | — | ApiResponse<string> | Remove empresa |

---

## 🔎 CNPJ

| Método | Rota | Auth | Controller | Request | Response | Obs. |
|------|------|------|------------|--------|---------|------|
| GET | `/api/cnpj/{cnpj}` | Contador | CnpjController.Get | — | ApiResponse<CnpjResponse> | Consulta externa |
| POST | `/api/empresas/cnpj` | Contador | EmpresasController.CreateFromCnpj | CnpjRequest | ApiResponse<EmpresaResponse> | Cadastro automático |

---

# [PLANEJADO]

## Auth

| Método | Rota |
|------|------|
| POST | `/api/auth/refresh` |

---

## Auditoria

| Método | Rota |
|------|------|
| GET | `/api/audit-log` |

---

## Configurações

| Método | Rota |
|------|------|
| GET | `/api/settings` |
| PUT | `/api/settings` |

---