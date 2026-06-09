import React, { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { Button } from '../../components/Button';
import { Input } from '../../components/Input';
import { perfilService } from '../../services/perfilService';
import { User, Key, Save, AlertCircle, CheckCircle2 } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

const profileSchema = z.object({
  nome: z.string().min(3, 'O nome deve ter pelo menos 3 caracteres'),
  email: z.string().email('Email inválido'),
});

const passwordSchema = z.object({
  senhaAtual: z.string().min(1, 'Senha atual é obrigatória'),
  novaSenha: z.string().min(6, 'A nova senha deve ter pelo menos 6 caracteres'),
  confirmarSenha: z.string().min(1, 'Confirmação de senha é obrigatória'),
}).refine((data) => data.novaSenha === data.confirmarSenha, {
  message: "As senhas não coincidem",
  path: ["confirmarSenha"],
});

type ProfileFormData = z.infer<typeof profileSchema>;
type PasswordFormData = z.infer<typeof passwordSchema>;

export const Perfil: React.FC = () => {
  const { user, updateUser } = useAuth();
  const [profileLoading, setProfileLoading] = useState(false);
  const [passwordLoading, setPasswordLoading] = useState(false);
  const [profileMessage, setProfileMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
  const [passwordMessage, setPasswordMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);

  const {
    register: registerProfile,
    handleSubmit: handleSubmitProfile,
    formState: { errors: profileErrors },
  } = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      nome: user?.nome || '',
      email: user?.email || '',
    },
  });

  const {
    register: registerPassword,
    handleSubmit: handleSubmitPassword,
    reset: resetPassword,
    formState: { errors: passwordErrors },
  } = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  });

  const onUpdateProfile = async (data: ProfileFormData) => {
    setProfileLoading(true);
    setProfileMessage(null);
    try {
      await perfilService.updatePerfil(data);
      if (user) {
        updateUser({ ...user, ...data });
      }
      setProfileMessage({ type: 'success', text: 'Perfil atualizado com sucesso!' });
    } catch (error: any) {
      setProfileMessage({
        type: 'error',
        text: error.response?.data?.mensagem || 'Erro ao atualizar perfil.'
      });
    } finally {
      setProfileLoading(false);
    }
  };

  const onChangePassword = async (data: PasswordFormData) => {
    setPasswordLoading(true);
    setPasswordMessage(null);
    try {
      await perfilService.alterarSenha({
        senhaAtual: data.senhaAtual,
        novaSenha: data.novaSenha,
      });
      setPasswordMessage({ type: 'success', text: 'Senha alterada com sucesso!' });
      resetPassword();
    } catch (error: any) {
      setPasswordMessage({
        type: 'error',
        text: error.response?.data?.mensagem || 'Erro ao alterar senha.'
      });
    } finally {
      setPasswordLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Meu Perfil</h1>
        <p className="text-gray-600">Gerencie suas informações pessoais e segurança da conta.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Dados Básicos */}
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center space-x-2 mb-6">
            <User className="h-5 w-5 text-blue-600" />
            <h2 className="text-lg font-semibold text-gray-800">Dados Pessoais</h2>
          </div>

          <form onSubmit={handleSubmitProfile(onUpdateProfile)} className="space-y-4">
            <Input
              label="Nome Completo"
              {...registerProfile('nome')}
              error={profileErrors.nome?.message}
            />
            <Input
              label="E-mail"
              type="email"
              {...registerProfile('email')}
              error={profileErrors.email?.message}
            />

            {profileMessage && (
              <div className={`p-3 rounded-md flex items-center space-x-2 text-sm ${
                profileMessage.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'
              }`}>
                {profileMessage.type === 'success' ? <CheckCircle2 className="h-4 w-4" /> : <AlertCircle className="h-4 w-4" />}
                <span>{profileMessage.text}</span>
              </div>
            )}

            <Button type="submit" className="w-full" isLoading={profileLoading}>
              <Save className="h-4 w-4 mr-2" />
              Salvar Alterações
            </Button>
          </form>
        </div>

        {/* Segurança / Troca de Senha */}
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center space-x-2 mb-6">
            <Key className="h-5 w-5 text-blue-600" />
            <h2 className="text-lg font-semibold text-gray-800">Segurança</h2>
          </div>

          <form onSubmit={handleSubmitPassword(onChangePassword)} className="space-y-4">
            <Input
              label="Senha Atual"
              type="password"
              {...registerPassword('senhaAtual')}
              error={passwordErrors.senhaAtual?.message}
            />
            <Input
              label="Nova Senha"
              type="password"
              {...registerPassword('novaSenha')}
              error={passwordErrors.novaSenha?.message}
            />
            <Input
              label="Confirmar Nova Senha"
              type="password"
              {...registerPassword('confirmarSenha')}
              error={passwordErrors.confirmarSenha?.message}
            />

            {passwordMessage && (
              <div className={`p-3 rounded-md flex items-center space-x-2 text-sm ${
                passwordMessage.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'
              }`}>
                {passwordMessage.type === 'success' ? <CheckCircle2 className="h-4 w-4" /> : <AlertCircle className="h-4 w-4" />}
                <span>{passwordMessage.text}</span>
              </div>
            )}

            <Button type="submit" variant="outline" className="w-full" isLoading={passwordLoading}>
              <Key className="h-4 w-4 mr-2" />
              Alterar Senha
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};
