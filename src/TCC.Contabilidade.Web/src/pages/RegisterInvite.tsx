import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { Input } from '../components/Input';
import { Button } from '../components/Button';
import { AlertCircle, CheckCircle2 } from 'lucide-react';
import { isAxiosError } from 'axios';

const registerSchema = z.object({
  nome: z.string().min(3, 'O nome deve ter pelo menos 3 caracteres'),
  email: z.string().email('E-mail inválido'),
  senha: z.string().min(6, 'A senha deve ter pelo menos 6 caracteres'),
  confirmarSenha: z.string().min(6, 'A confirmação de senha é obrigatória'),
}).refine((data) => data.senha === data.confirmarSenha, {
  message: "As senhas não coincidem",
  path: ["confirmarSenha"],
});

type RegisterFormData = z.infer<typeof registerSchema>;

export const RegisterInvite: React.FC = () => {
  const { signUpWithInvite } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [success, setSuccess] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const token = searchParams.get('token');
  const [error, setError] = useState<string | null>(token ? null : 'Token de convite não encontrado ou inválido.');

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormData) => {
    if (!token) return;

    try {
      setIsLoading(true);
      setError(null);
      await signUpWithInvite({
        invitationToken: token,
        nome: data.nome,
        email: data.email,
        senha: data.senha,
      });
      setSuccess(true);
      setTimeout(() => navigate('/login'), 3000);
    } catch (err: unknown) {
      if (isAxiosError(err)) {
        setError(err.response?.data?.mensagem || 'Erro ao realizar cadastro. O convite pode ter expirado.');
      } else {
        setError('Ocorreu um erro inesperado.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  if (success) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-100 px-4">
        <div className="w-full max-w-md space-y-4 rounded-lg bg-white p-8 shadow-md text-center">
          <div className="flex justify-center">
            <CheckCircle2 className="h-16 w-16 text-green-500" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900">Cadastro realizado!</h2>
          <p className="text-gray-600">Seu cadastro foi concluído com sucesso. Você será redirecionado para a tela de login em instantes.</p>
          <Button onClick={() => navigate('/login')} className="w-full mt-4">
            Ir para Login
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 px-4">
      <div className="w-full max-w-md space-y-8 rounded-lg bg-white p-8 shadow-md">
        <div className="text-center">
          <h2 className="text-3xl font-bold tracking-tight text-gray-900">Finalizar Cadastro</h2>
          <p className="mt-2 text-sm text-gray-600">Preencha seus dados para acessar o sistema</p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
          {error && (
            <div className="flex items-center space-x-2 rounded-md bg-red-50 p-4 text-sm text-red-700">
              <AlertCircle className="h-5 w-5" />
              <span>{error}</span>
            </div>
          )}

          <div className="space-y-4">
            <Input
              label="Nome Completo"
              type="text"
              error={errors.nome?.message}
              {...register('nome')}
              disabled={!token}
            />

            <Input
              label="E-mail"
              type="email"
              error={errors.email?.message}
              {...register('email')}
              disabled={!token}
            />

            <Input
              label="Senha"
              type="password"
              error={errors.senha?.message}
              {...register('senha')}
              disabled={!token}
            />

            <Input
              label="Confirmar Senha"
              type="password"
              error={errors.confirmarSenha?.message}
              {...register('confirmarSenha')}
              disabled={!token}
            />
          </div>

          <Button type="submit" className="w-full" isLoading={isLoading} disabled={!token}>
            Criar Conta
          </Button>
        </form>
      </div>
    </div>
  );
};
