

<h1 align="center">📊 TCC — Plataforma de Gestão Contábil Multi-Tenant</h1>

<p align="center">
Sistema backend desenvolvido como Trabalho de Conclusão de Curso (TCC),
focado em arquitetura escalável, segurança e organização profissional de software.
</p>

<hr>

<h2>🎯 Objetivo do Projeto</h2>

<p>
Desenvolver uma plataforma backend para gestão contábil que permita
a administração de múltiplas empresas por contadores, utilizando
uma arquitetura segura, escalável e preparada para integrações externas.
</p>

<p>
O projeto adota práticas modernas de engenharia de software,
incluindo separação em camadas, controle de acesso, auditoria
de ações e suporte a arquitetura <strong>multi-tenant</strong>.
</p>

<hr>

<h2>🏗 Arquitetura do Sistema</h2>

<p>O sistema segue uma arquitetura em camadas baseada em boas práticas de desenvolvimento backend.</p>

<pre>
API
│
├── Controllers
├── Middlewares
│
Application
│
├── Services
├── DTOs
├── Interfaces
│
Domain
│
├── Entities
│
Infrastructure
│
├── Repositories
├── Integrations
├── Cache
</pre>

<p>
Essa arquitetura promove <strong>baixo acoplamento</strong>, 
<strong>facilidade de manutenção</strong> e <strong>escalabilidade</strong>.
</p>

<hr>

<h2>🔐 Segurança da Aplicação</h2>

<ul>

<li>Autenticação baseada em <strong>JWT (JSON Web Token)</strong></li>

<li>Controle de acesso utilizando <strong>RBAC (Role-Based Access Control)</strong></li>

<li>Sistema de <strong>registro por convite</strong></li>

<li>Expiração automática de convites</li>

<li>Proteção contra <strong>IDOR (Insecure Direct Object Reference)</strong></li>

<li><strong>Rate Limiting</strong> para proteção contra abuso de API</li>

<li><strong>Refresh Token</strong> para controle de sessões</li>

<li>Sistema de <strong>Audit Log</strong> para rastreabilidade de ações</li>

</ul>

<hr>

<h2>🏢 Arquitetura Multi-Tenant</h2>

<p>
A aplicação foi projetada para suportar múltiplas empresas
utilizando o mesmo sistema, garantindo isolamento lógico de dados.
</p>

<p>
Cada requisição é executada dentro de um contexto de <strong>Tenant</strong>,
identificado automaticamente através do token de autenticação.
</p>

<ul>

<li>Isolamento de dados entre empresas</li>

<li>Configuração personalizada por empresa</li>

<li>Controle de acesso baseado em roles</li>

</ul>

<hr>

<h2>⚡ Performance e Escalabilidade</h2>

<ul>

<li>Paginação em endpoints de listagem</li>

<li>Uso de <strong>AsNoTracking()</strong> para consultas de leitura</li>

<li>Implementação de <strong>Cache em memória</strong></li>

<li>Arquitetura preparada para <strong>Redis</strong></li>

<li>Integrações externas desacopladas</li>

</ul>

<hr>

<h2>🔌 Integrações Externas</h2>

<p>
A plataforma possui uma camada dedicada de integração para comunicação
com APIs externas.
</p>

<p>Exemplos de integrações planejadas:</p>

<ul>

<li>Consulta de CNPJ</li>

<li>Integração com ERPs contábeis</li>

<li>Serviços fiscais</li>

<li>APIs de dados empresariais</li>

</ul>

<hr>

<h2>📋 Gerenciamento do Projeto</h2>

<p>
O desenvolvimento do sistema é organizado utilizando recursos do GitHub:
</p>

<ul>

<li><strong>Issues</strong> para planejamento técnico</li>

<li><strong>Pull Requests</strong> para implementação de funcionalidades</li>

<li>Documentação de arquitetura</li>

<li>Critérios de aceite para cada tarefa</li>

</ul>

<hr>

<h2>⚙️ Pipeline CI/CD</h2>

<p>
O projeto utiliza <strong>GitHub Actions</strong> para garantir a qualidade e estabilidade do código.
Toda <strong>Pull Request</strong> aberta para a branch <code>main</code> dispara automaticamente uma pipeline de validação que executa as seguintes etapas:
</p>

<ul>
  <li><strong>Restore:</strong> Restauração de dependências do NuGet.</li>
  <li><strong>Build:</strong> Compilação da solução para garantir que não existam erros de sintaxe ou referências quebradas.</li>
</ul>

<p>
A aprovação do merge está condicionada ao sucesso dessa pipeline, reduzindo o risco de introdução de regressões no ambiente principal.
</p>

<hr>

<h2>🚀 Tecnologias Utilizadas</h2>

<ul>

<li>.NET / ASP.NET Core</li>

<li>Entity Framework Core</li>

<li>SQL Server</li>

<li>JWT Authentication</li>

<li>Swagger / OpenAPI</li>

<li>Git / GitHub</li>

</ul>

<hr>

<h2>📊 Funcionalidades Principais</h2>

<ul>

<li>Autenticação de usuários</li>

<li>Controle de acesso por roles (RBAC)</li>

<li>Sistema de convites para cadastro</li>

<li>Gestão de empresas</li>

<li>Arquitetura multi-tenant</li>

<li>Logs de auditoria</li>

<li>Integração com APIs externas</li>

</ul>

<hr>

<h2>👨‍💻 Desenvolvimento</h2>

<p>
Este projeto está sendo desenvolvido como parte do
<strong>Trabalho de Conclusão de Curso (TCC)</strong>,
com foco na aplicação de conceitos de engenharia de software,
segurança e arquitetura de sistemas.
</p>

<hr>

<h2>📌 Status do Projeto</h2>

<p>
🚧 Em desenvolvimento
</p>

<p>
Novas funcionalidades estão sendo planejadas e implementadas
seguindo um fluxo organizado de desenvolvimento.
</p>
