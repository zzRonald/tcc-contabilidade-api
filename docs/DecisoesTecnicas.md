# Decisões Técnicas — TCC Contabilidade API

> **Fonte da verdade:** código atual do projeto
>
> **Status**
> - **[IMPLEMENTADO]** = já aplicado no código
> - **[PLANEJADO]** = definido mas ainda não implementado

---

# [IMPLEMENTADO] Arquitetura e Estrutura

## DT-001 — Arquitetura em camadas (Clean Architecture)

**Status:** [IMPLEMENTADO]

**Decisão:**
Separação do sistema em camadas:

- API (Controllers)
- Application (Services)
- Domain (Entidades)
- Infrastructure (Persistência e integrações)

**Motivação:**
- Separação de responsabilidades
- Facilidade de manutenção
- Testabilidade

**Implicações:**
- Controllers não acessam diretamente o banco
- Regras ficam centralizadas na Application/Domain

---

## DT-002 — Uso de DTOs para comunicação

**Status:** [IMPLEMENTADO]

**Decisão:**
Utilizar DTOs para entrada e saída da API

**Motivação:**
- Evitar exposição de entidades do domínio
- Controle de contratos da API

**Implicações:**
- Mapeamento entre DTO ↔ Domain
- Facilidade de versionamento futuro

---

## DT-003 — Controllers finos + Services

**Status:** [IMPLEMENTADO]

**Decisão:**
Controllers apenas orquestram requisições e delegam para services

**Motivação:**
- Manter regras fora da camada HTTP
- Melhor organização do código

---

# [IMPLEMENTADO] Segurança

## DT-010 — Autenticação com JWT

**Status:** [IMPLEMENTADO]

**Decisão:**
Utilizar JWT para autenticação dos usuários

**Motivação:**
- Padrão amplamente utilizado
- Stateless
- Fácil integração com front-end

**Implicações:**
- Token enviado via Authorization header
- Expiração controlada

---

## DT-011 — Autorização baseada em roles (RBAC)

**Status:** [IMPLEMENTADO]

**Decisão:**
Controle de acesso baseado em roles:

- Admin
- Contador
- Usuário

**Motivação:**
- Flexibilidade de permissões
- Escalabilidade do sistema

**Implicações:**
- Uso de `[Authorize]` com roles/policies
- Centralização de regras de acesso

---

## DT-012 — Sistema de convites com token

**Status:** [IMPLEMENTADO]

**Decisão:**
Cadastro de usuários via convite com token

**Motivação:**
- Controle de acesso ao sistema
- Segurança na criação de contas

**Implicações:**
- Tokens precisam expirar
- Validação obrigatória no registro

---

## DT-013 — Expiração de convites

**Status:** [IMPLEMENTADO]

**Decisão:**
Convites possuem data de validade

**Motivação:**
- Evitar uso indevido
- Aumentar segurança

---

## DT-014 — Rate Limiting

**Status:** [IMPLEMENTADO]

**Decisão:**
Limitar requisições em endpoints sensíveis

**Motivação:**
- Prevenção contra brute force
- Proteção contra DoS

**Implicações:**
- Resposta HTTP 429
- Controle por IP/usuário

---

# [IMPLEMENTADO] Padronização da API

## DT-020 — ApiResponse padrão

**Status:** [IMPLEMENTADO]

**Decisão:**
Todas respostas seguem padrão único:

- status
- message
- data
- errors

**Motivação:**
- Padronização
- Facilidade para front-end
- Melhor debug

---

## DT-021 — Tratamento centralizado de erros

**Status:** [IMPLEMENTADO]

**Decisão:**
Uso de middleware/global handler para exceções

**Motivação:**
- Evitar try/catch em controllers
- Padronizar respostas de erro

---

# [IMPLEMENTADO] Integrações

## DT-030 — Integração com API de CNPJ

**Status:** [IMPLEMENTADO]

**Decisão:**
Consumir API externa para obter dados de empresas via CNPJ

**Motivação:**
- Automatizar cadastro
- Reduzir erro humano

**Implicações:**
- Dependência de serviço externo
- Necessidade de tratamento de falhas

---

## DT-031 — Cadastro inteligente de empresa

**Status:** [IMPLEMENTADO]

**Decisão:**
Preencher automaticamente dados da empresa via CNPJ

**Motivação:**
- Melhor experiência do usuário
- Redução de esforço manual

---

# [IMPLEMENTADO] Persistência

## DT-040 — Uso de ORM (Entity Framework)

**Status:** [IMPLEMENTADO]

**Decisão:**
Utilizar Entity Framework para acesso a dados

**Motivação:**
- Produtividade
- Integração com .NET

**Implicações:**
- Uso de migrations
- Abstração do banco

---

## DT-041 — Migrations para versionamento

**Status:** [IMPLEMENTADO]

**Decisão:**
Controlar alterações do banco via migrations

**Motivação:**
- Versionamento do schema
- Facilidade de deploy

---

# [PLANEJADO] Arquitetura e Escalabilidade

## DT-100 — Multi-Tenant

**Status:** [PLANEJADO]

**Decisão:**
Separar dados por empresa (tenant)

**Motivação:**
- Escalabilidade
- Isolamento de dados

---

## DT-101 — Camada de integração externa

**Status:** [PLANEJADO]

**Decisão:**
Criar camada dedicada para integrações

**Motivação:**
- Melhor organização
- Reuso de integrações

---

## DT-102 — Cache de dados

**Status:** [PLANEJADO]

**Decisão:**
Implementar cache para otimizar performance

**Motivação:**
- Reduzir chamadas externas
- Melhorar tempo de resposta

---

# [PLANEJADO] Segurança

## DT-110 — Refresh Token

**Status:** [PLANEJADO]

**Decisão:**
Implementar refresh token

**Motivação:**
- Melhor experiência do usuário
- Segurança

---

# [PLANEJADO] Observabilidade

## DT-120 — Auditoria de ações

**Status:** [PLANEJADO]

**Decisão:**
Registrar ações dos usuários

**Motivação:**
- Rastreabilidade
- Segurança

---

## DT-121 — Logging estruturado

**Status:** [PLANEJADO]

**Decisão:**
Implementar logs estruturados

**Motivação:**
- Debug eficiente
- Monitoramento

---

# [PLANEJADO] Performance

## DT-130 — Paginação de endpoints

**Status:** [PLANEJADO]

**Decisão:**
Paginar listagens

**Motivação:**
- Performance
- Escalabilidade

---