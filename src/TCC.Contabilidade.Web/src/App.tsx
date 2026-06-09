import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { PublicRoute } from './components/PublicRoute';
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { RegisterInvite } from './pages/RegisterInvite';
import { EmpresaList } from './pages/Empresas/EmpresaList';
import { EmpresaForm } from './pages/Empresas/EmpresaForm';
import { ConviteList } from './pages/Convites/ConviteList';
import { Perfil } from './pages/Perfil/Perfil';
import { Configuracoes } from './pages/Configuracoes/Configuracoes';
import { Layout } from './components/Layout/Layout';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Rotas Públicas */}
          <Route element={<PublicRoute />}>
            <Route path="/login" element={<Login />} />
            <Route path="/register-invite" element={<RegisterInvite />} />
          </Route>

          {/* Rotas Protegidas */}
          <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
              <Route path="/" element={<Home />} />
              <Route path="/perfil" element={<Perfil />} />

              {/* Gestão de Empresas - Apenas Contador ou Admin */}
              <Route element={<ProtectedRoute allowedRoles={['Contador', 'Admin']} />}>
                <Route path="/empresas" element={<EmpresaList />} />
                <Route path="/empresas/nova" element={<EmpresaForm />} />
                <Route path="/empresas/editar/:id" element={<EmpresaForm />} />
              </Route>

              {/* Painel de Convites - Apenas Contador */}
              <Route element={<ProtectedRoute allowedRoles={['Contador']} />}>
                <Route path="/convites" element={<ConviteList />} />
              </Route>

              {/* Configurações - Apenas Contador ou Admin */}
              <Route element={<ProtectedRoute allowedRoles={['Contador', 'Admin']} />}>
                <Route path="/configuracoes" element={<Configuracoes />} />
              </Route>
            </Route>
          </Route>

          {/* Redirecionamento padrão */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
