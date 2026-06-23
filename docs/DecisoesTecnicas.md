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

## DT-004 — Multi-Tenant (Isolamento de Dados)

**Status:** [IMPLEMENTADO]

**Decisão:**
Implementação de arquitetura multi-tenant utilizando identificação via Token JWT e filtros globais no EF Core.

**Motivação:**
- Garantir isolamento lógico de dados entre diferentes empresas (clientes).
- Segurança e escalabilidade.

**Implicações:**
- Middleware extrai `tenantId` do claim do token.
- `AppDbContext` aplica `HasQueryFilter` em todas as entidades que implementam `ITenantEntity`.

---

## DT-005 — Camada de integração externa

**Status:** [IMPLEMENTADO]

**Decisão:**
Criação de serviços especializados para consumo de APIs externas (ex: ReceitaWS para CNPJ).

**Motivação:**
- Desacoplamento de serviços externos.
- Facilidade para mockar em testes.

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

## DT-015 — Refresh Token

**Status:** [IMPLEMENTADO]

**Decisão:**
Implementação de Refresh Tokens para renovação de sessões sem necessidade de novo login manual.

**Motivação:**
- Melhorar a experiência do usuário (UX).
- Manter segurança com tokens de acesso de curta duração.

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

# [IMPLEMENTADO] Observabilidade e Performance

## DT-050 — Auditoria de ações (Audit Log)

**Status:** [IMPLEMENTADO]

**Decisão:**
Middleware que intercepta requisições de escrita para persistir trilha de auditoria.

**Motivação:**
- Rastreabilidade de ações críticas (quem, quando, o quê).
- Conformidade e segurança.

---

## DT-051 — Cache de dados em memória

**Status:** [IMPLEMENTADO]

**Decisão:**
Uso de `IMemoryCache` para dados consultados frequentemente e que mudam pouco (ex: configurações).

**Motivação:**
- Reduzir latência e carga no banco de dados.

---

## DT-052 — Paginação de listagens

**Status:** [IMPLEMENTADO]

**Decisão:**
Implementação de paginação (skip/take) em endpoints de busca e listagem.

**Motivação:**
- Performance e economia de recursos (memória/banda).

---

# [PLANEJADO] Próximos Passos

## DT-200 — Exportação de dados (LGPD)

**Status:** [PLANEJADO]

**Decisão:**
Facilitar a exportação completa de dados do usuário em formato legível.

**Motivação:**
- Conformidade total com LGPD.

---

## DT-201 — Dashboard Avançado com Gráficos

**Status:** [PLANEJADO]

**Decisão:**
Implementar métricas visuais e comparativos mensais.

**Motivação:**
- Valor agregado para o usuário final.

---
