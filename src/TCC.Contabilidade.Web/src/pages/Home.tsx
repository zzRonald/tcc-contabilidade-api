export const Home = () => {
  return (
    <div className="min-h-screen bg-gray-100 flex flex-col items-center justify-center p-4">
      <div className="bg-white p-8 rounded-lg shadow-md max-w-md w-full text-center">
        <h1 className="text-3xl font-bold text-blue-600 mb-4">
          📊 TCC Contabilidade
        </h1>
        <p className="text-gray-600 mb-6">
          Bem-vindo à plataforma de gestão contábil. Setup inicial concluído com sucesso!
        </p>
        <div className="space-y-2 text-sm text-left">
          <p className="font-semibold text-gray-700">O que foi configurado:</p>
          <ul className="list-disc list-inside text-gray-600 space-y-1">
            <li>Vite + React + TypeScript</li>
            <li>Tailwind CSS</li>
            <li>Estrutura de pastas</li>
            <li>Axios (API Client)</li>
            <li>Variáveis de ambiente</li>
          </ul>
        </div>
      </div>
    </div>
  );
};
