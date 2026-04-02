# Casos de Uso — TCC Contabilidade API

> **Fonte da verdade:** código atual (Controllers + Services + Domain)
>
> **Status**
> - **[IMPLEMENTADO]** = já existe no código
> - **[PLANEJADO]** = backlog / não implementado ainda

---

## Atores do Sistema

- **SystemAdmin** → administrador do sistema
- **Contador** → responsável por gerenciar empresas
- **Usuário Convidado** → entra via convite
- **Sistema Externo (CNPJ API)** → integração externa

---

# [IMPLEMENTADO] Autenticação e Acesso

## UC-001 — Registro com convite 

**Atores:** Usuário convidado  
**Pré-condições:** Possuir token de convite válido  
**Fluxo principal:**
1. Usuário acessa link com token
2. Envia dados de cadastro
3. API valida token
4. API cria usuário
5. API associa ao contador/empresa

**Fluxos alternativos:**
- Token expirado → **400**
- Token inválido → **401**

**Endpoints:**
- `POST /auth/register-with-invite`

---

## UC-002 — Login com JWT

**Atores:** Usuário  
**Fluxo principal:**
1. Usuário envia email/senha
2. API valida credenciais
3. API retorna JWT

**Fluxos alternativos:**
- Credenciais inválidas → **401**
- Rate limit excedido → **429**

**Endpoints:**
- `POST /auth/login`

---

## UC-003 — Controle de acesso por roles (RBAC)

**Atores:** SystemAdmin  
**Descrição:**
Sistema controla permissões por roles:

- Admin
- Contador
- Usuário

**Fluxo principal:**
1. Usuário acessa endpoint
2. API valida role
3. Permite ou bloqueia acesso

---

## UC-004 — Convites para novos usuários

**Atores:** Contador  
**Fluxo principal:**
1. Contador cria convite
2. Sistema gera token
3. Usuário recebe acesso

**Fluxos alternativos:**
- Convite expirado → inválido

**Endpoints:**
- `POST /invites`
- `GET /invites`

---

## UC-005 — Expiração de convites

**Atores:** Sistema  
**Fluxo principal:**
1. Sistema valida data de expiração
2. Bloqueia uso de convites antigos

---

# [IMPLEMENTADO] Empresas

## UC-010 — Criar empresa

**Atores:** Contador  
**Fluxo principal:**
1. Usuário envia dados da empresa
2. API valida informações
3. Empresa é criada

**Endpoints:**
- `POST /empresas`

---

## UC-011 — Listar empresas

**Atores:** Contador  
**Fluxo principal:**
1. Usuário consulta empresas
2. API retorna lista

**Endpoints:**
- `GET /empresas`

---

## UC-012 — Atualizar empresa

**Atores:** Contador  
**Fluxo principal:**
1. Usuário envia alterações
2. API valida e atualiza

**Endpoints:**
- `PUT /empresas/{id}`

---

## UC-013 — Remover empresa

**Atores:** Contador  
**Fluxo principal:**
1. Usuário solicita remoção
2. API remove registro

**Endpoints:**
- `DELETE /empresas/{id}`

---

# [IMPLEMENTADO] Integração com CNPJ

## UC-020 — Consulta de CNPJ

**Atores:** Contador  
**Pré-condições:** CNPJ válido  

**Fluxo principal:**
1. Usuário informa CNPJ
2. API chama serviço externo
3. Dados da empresa são retornados

**Fluxos alternativos:**
- CNPJ inválido → **400**
- API externa indisponível → **503**

**Endpoints:**
- `GET /cnpj/{cnpj}`

---

## UC-021 — Cadastro inteligente de empresa

**Atores:** Contador  
**Fluxo principal:**
1. Usuário informa CNPJ
2. Sistema busca dados automaticamente
3. Empresa é criada com dados preenchidos

---

# [IMPLEMENTADO] Estrutura da API

## UC-030 — Padronização de respostas

**Atores:** Sistema  

**Descrição:**
Todas respostas seguem padrão:

- status
- message
- data
- errors

---

## UC-031 — Proteção contra ataques (Rate Limiting)

**Atores:** Sistema  

**Fluxo principal:**
1. Sistema monitora requisições
2. Limita excesso de chamadas
3. Retorna erro 429

---

# [PLANEJADO] Funcionalidades Futuras

## UC-100 — Refresh Token

**Descrição:**
Permitir renovação de token sem novo login

---

## UC-101 — Auditoria de ações (Audit Log)

**Descrição:**
Registrar ações do usuário no sistema

---

## UC-102 — Multi-Tenant (Multi empresa por usuário)

**Descrição:**
Permitir múltiplas empresas por contexto isolado

---

## UC-103 — Configurações por empresa

**Descrição:**
Cada empresa terá configurações próprias

---

## UC-104 — Camada de integração externa

**Descrição:**
Centralizar integrações com APIs externas

---

## UC-105 — Cache de dados

**Descrição:**
Melhorar performance com cache

---

# [PLANEJADO] Melhorias Técnicas

- Middleware de Tenant
- Sistema de logs estruturados
- Melhor tratamento de erros
- Paginação nos endpoints