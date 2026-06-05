# 📊 TCC Contabilidade — Frontend

Este é o frontend da plataforma de gestão contábil, desenvolvido com **React**, **TypeScript** e **Vite**.

## 🚀 Tecnologias

- [React](https://reactjs.org/)
- [TypeScript](https://www.typescriptlang.org/)
- [Vite](https://vitejs.dev/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Axios](https://axios-http.com/)

## 📁 Estrutura de Pastas

A estrutura segue um padrão modular para facilitar a manutenção e escalabilidade:

```text
src/
├── api/          # Configuração do Axios e instâncias de API
├── assets/       # Imagens, ícones e arquivos estáticos
├── components/   # Componentes reutilizáveis
├── contexts/     # Contextos do React (Auth, Theme, etc.)
├── hooks/        # Custom hooks
├── interfaces/   # Definições de tipos e interfaces TypeScript
├── pages/        # Componentes de página (rotas)
├── services/     # Lógica de consumo de APIs externas e serviços
├── styles/       # Arquivos de estilo globais
└── utils/        # Funções utilitárias
```

## ⚙️ Configuração

### Pré-requisitos

- Node.js (v18+)
- npm ou yarn

### Instalação

1. Entre na pasta do frontend:
   ```bash
   cd src/TCC.Contabilidade.Web
   ```

2. Instale as dependências:
   ```bash
   npm install
   ```

3. Configure as variáveis de ambiente:
   - Copie o arquivo `.env.example` para `.env`
   - Ajuste a URL da API se necessário (`VITE_API_URL`)

### Execução

Para rodar o projeto em modo de desenvolvimento:

```bash
npm run dev
```

### Build

Para gerar a versão de produção:

```bash
npm run build
```

## 🛠 Padrões de Desenvolvimento

- **Componentes:** Devem ser criados na pasta `components` se forem reutilizáveis, ou dentro da pasta da página específica se forem locais.
- **Estilização:** Utilizar classes do Tailwind CSS sempre que possível.
- **Tipagem:** Manter interfaces atualizadas em `src/interfaces` para garantir segurança com TypeScript.
- **API:** Todas as chamadas para o backend devem utilizar a instância do `apiClient` definida em `src/api/axios.ts`.
