# Política de Privacidade e Tratamento de Dados (LGPD)

Este documento descreve os dados pessoais tratados pelo sistema **TCC Contabilidade** em conformidade com a Lei Geral de Proteção de Dados (Lei nº 13.709/2018).

## 1. Dados Coletados e Finalidades

| Dado Pessoal | Categoria | Finalidade | Base Legal |
| :--- | :--- | :--- | :--- |
| Nome | Identificação | Identificar o usuário no sistema e em comunicações. | Execução de Contrato |
| E-mail | Contato | Login, recuperação de senha e notificações do sistema. | Execução de Contrato |
| Senha (Hash) | Autenticação | Garantir acesso seguro ao sistema. | Execução de Contrato |
| IP | Registro | Auditoria de segurança e prevenção a fraudes. | Legítimo Interesse |
| CPF/CNPJ | Identificação Fiscal | Identificação de clientes e funcionários para obrigações contábeis. | Cumprimento de Obrigação Legal |

## 2. Minimização de Dados

O sistema adota o princípio de *privacy by design*, aplicando as seguintes técnicas:
- **Mascaramento**: E-mails, IPs e CNPJs são mascarados em listagens e interfaces administrativas para evitar exposição desnecessária.
- **Hashing**: Senhas nunca são armazenadas em texto claro, utilizando algoritmos robustos (BCrypt).
- **Limitação de Acesso**: O acesso aos dados é restrito com base no perfil do usuário (Contador, Cliente, Admin).

## 3. Direitos do Titular

Em conformidade com a LGPD, o usuário possui os seguintes direitos:
- **Confirmação e Acesso**: Saber se seus dados são tratados e acessá-los.
- **Correção**: Solicitar alteração de dados incompletos ou inexatos.
- **Exportação (Portabilidade)**: O sistema disponibiliza um endpoint (`/api/Perfil/exportar-dados`) para exportação dos dados pessoais em formato estruturado.
- **Exclusão**: Direito de solicitar a inativação ou exclusão de sua conta (sujeito a obrigações legais de retenção).

## 4. Segurança

Utilizamos medidas técnicas e administrativas para proteger os dados pessoais contra acessos não autorizados e situações acidentais ou ilícitas de destruição, perda, alteração ou comunicação.
- Cabeçalhos de segurança (X-Frame-Options, CSP, etc.).
- Proteção contra Cross-Site Scripting (XSS).
- Auditoria de ações críticas no sistema.
