# 🚀 Backlog de Tarefas — TCC Contabilidade

Este documento contém o planejamento de issues necessárias para a conclusão do sistema, organizadas por categorias.

---

## 🏗 Infraestrutura `[Infra]`

### 1. [Infra] Setup de CI/CD com GitHub Actions
**Descrição:** Configurar automação para garantir a qualidade do código a cada push/pull request.
- **Critérios de Aceite:**
    - Workflow de `build` executando em cada PR.
    - Workflow de `tests` garantindo que nenhuma alteração quebre o sistema.
    - (Opcional) Deploy automático em ambiente de staging.

### 2. [Infra] Migração para Redis Cache
**Descrição:** Substituir a implementação atual de `IMemoryCache` por um provedor distribuído usando Redis.
- **Critérios de Aceite:**
    - Docker Compose atualizado com imagem do Redis.
    - Implementação do `ICacheService` utilizando `StackExchange.Redis`.
    - Cache mantido funcional mesmo após restart da API.

### 3. [Infra] Dockerização da Solução
**Descrição:** Criar arquivos de containerização para padronizar o ambiente de desenvolvimento e produção.
- **Critérios de Aceite:**
    - `Dockerfile` otimizado para a API (.NET 8/9).
    - `docker-compose.yml` orquestrando API, SQL Server e Redis.
    - README atualizado com instruções de `docker-compose up`.

---

## 🔐 Segurança `[Seg]`

### 4. [Seg] Fluxo de Recuperação de Senha
**Descrição:** Implementar o fluxo completo de "Esqueci minha senha".
- **Critérios de Aceite:**
    - Endpoint para solicitar recuperação (gera token temporário).
    - Serviço de envio de e-mail integrado.
    - Endpoint para reset de senha validando o token.

### 5. [Seg] Políticas de CORS e Segurança de Cabeçalhos
**Descrição:** Configurar a API para aceitar requisições apenas de origens confiáveis e adicionar camadas de proteção HTTP.
- **Critérios de Aceite:**
    - Middleware de CORS configurado com lista de origens permitidas.
    - Implementação de headers de segurança (X-Content-Type-Options, X-Frame-Options).

### 6. [Seg] Implementação de Multi-Factor Authentication (MFA)
**Descrição:** Adicionar suporte a autenticação em duas etapas via TOTP (Google Authenticator/Authy).
- **Critérios de Aceite:**
    - Endpoint para gerar QR Code de ativação.
    - Validação de código MFA durante o login.

---

## 🎯 Épicos `{ EPIC }`

### 7. { EPIC } Notificações do Sistema
**Descrição:** Centralizar o envio de comunicações do sistema (E-mail, Push, In-app).
- **Critérios de Aceite:**
    - Abstração `IEmailService` implementada (SMTP ou SendGrid).
    - Notificação enviada ao criar novo convite.
    - Registro de notificações enviadas no banco de dados.

### 8. { EPIC } Dashboard e Relatórios Gerenciais
**Descrição:** Fornecer uma visão analítica dos dados para o Contador e Admin.
- **Critérios de Aceite:**
    - Endpoint de "Resumo" com total de empresas, usuários e convites pendentes.
    - Relatório de atividades recentes baseado nos logs de auditoria.

### 9. { EPIC } Auditoria Pública para Contadores
**Descrição:** Expor os logs de auditoria para que os contadores possam monitorar ações em suas empresas.
- **Critérios de Aceite:**
    - `AuditController` criado com endpoint `GET /api/audit-log`.
    - Filtros por data, usuário e tipo de ação.
    - Garantia de que o contador só veja logs de sua própria "empresa context".

---

## 💻 Frontend `<FrontEnd>`

### 10. <FrontEnd> Setup Inicial e Arquitetura
**Descrição:** Iniciar o projeto frontend e definir padrões.
- **Critérios de Aceite:**
    - Projeto React + Vite + TypeScript configurado.
    - Tailwind CSS e biblioteca de componentes (Shadcn/UI) instalados.
    - Estrutura de pastas (components, hooks, services, pages).

### 11. <FrontEnd> Fluxo de Autenticação
**Descrição:** Telas necessárias para entrada no sistema.
- **Critérios de Aceite:**
    - Tela de Login integrada com `/api/auth/login`.
    - Persistência do JWT e Refresh Token no LocalStorage/Cookies.
    - Tela de Registro via Convite (validando token da URL).

### 12. <FrontEnd> Gestão de Empresas (CRUD)
**Descrição:** Interface para o contador gerenciar sua carteira de clientes.
- **Critérios de Aceite:**
    - Listagem de empresas com busca e paginação.
    - Formulário de cadastro com busca automática via CNPJ (integração com `/api/cnpj/{cnpj}`).
    - Edição e exclusão de empresas.

### 13. <FrontEnd> Painel de Convites
**Descrição:** Interface para enviar e monitorar convites de novos usuários.
- **Critérios de Aceite:**
    - Modal/Formulário para disparar novos convites por e-mail.
    - Tabela de convites com status (Pendente, Aceito, Expirado).

### 14. <FrontEnd> Configurações e Perfil
**Descrição:** Ajustes do sistema e dados do usuário.
- **Critérios de Aceite:**
    - Tela de perfil para troca de senha e dados pessoais.
    - Tela de configurações da empresa (moeda, fuso horário, preferências).

### 15. <FrontEnd> Visualizador de Auditoria
**Descrição:** Interface para rastreabilidade.
- **Critérios de Aceite:**
    - Tabela de logs com filtros avançados.
    - Visualização amigável de "Quem fez o quê e quando".
