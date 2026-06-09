import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { X, Mail, AlertCircle, CheckCircle2 } from 'lucide-react';
import { Button } from '../../components/Button';
import { conviteService } from '../../services/conviteService';
import { cn } from '../../utils/cn';

const conviteSchema = z.object({
  emailCliente: z.string().email('E-mail inválido'),
});

type ConviteFormData = z.infer<typeof conviteSchema>;

interface ConviteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const ConviteModal: React.FC<ConviteModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const [successData, setSuccessData] = React.useState<{ mensagem: string; token: string } | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ConviteFormData>({
    resolver: zodResolver(conviteSchema),
  });

  if (!isOpen) return null;

  const onSubmit = async (data: ConviteFormData) => {
    try {
      setLoading(true);
      setError(null);
      const response = await conviteService.criarConvite(data);
      setSuccessData(response);
    } catch (err: any) {
      setError(err.response?.data?.mensagem || 'Erro ao enviar convite. Tente novamente.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    reset();
    setError(null);
    setSuccessData(null);
    onClose();
  };

  const handleDone = () => {
    handleClose();
    onSuccess();
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4 text-center sm:p-0">
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" onClick={handleClose}></div>

        <div className="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg">
          <div className="absolute right-0 top-0 hidden pr-4 pt-4 sm:block">
            <button
              type="button"
              className="rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none"
              onClick={handleClose}
            >
              <X className="h-6 w-6" />
            </button>
          </div>

          <div className="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
            <div className="sm:flex sm:items-start">
              <div className={cn(
                "mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full sm:mx-0 sm:h-10 sm:w-10",
                successData ? "bg-green-100" : "bg-blue-100"
              )}>
                {successData ? (
                  <CheckCircle2 className="h-6 w-6 text-green-600" />
                ) : (
                  <Mail className="h-6 w-6 text-blue-600" />
                )}
              </div>
              <div className="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left w-full">
                <h3 className="text-xl font-semibold leading-6 text-gray-900">
                  {successData ? 'Convite Enviado!' : 'Novo Convite'}
                </h3>
                <div className="mt-2">
                  {successData ? (
                    <div className="space-y-4">
                      <p className="text-sm text-gray-500">
                        {successData.mensagem}
                      </p>
                      <div className="bg-gray-50 p-3 rounded border border-gray-200">
                        <p className="text-xs text-gray-500 uppercase font-bold mb-1">Token de Acesso:</p>
                        <code className="text-sm font-mono break-all text-blue-700">{successData.token}</code>
                      </div>
                      <p className="text-xs text-amber-600">
                        Dica: O cliente precisará deste token para completar o cadastro.
                      </p>
                    </div>
                  ) : (
                    <p className="text-sm text-gray-500">
                      Informe o e-mail do cliente que deseja convidar para o sistema.
                      Um convite será gerado e poderá ser utilizado para o cadastro.
                    </p>
                  )}
                </div>

                {!successData && (
                  <form onSubmit={handleSubmit(onSubmit)} className="mt-6 space-y-4">
                    <div>
                      <label htmlFor="emailCliente" className="block text-sm font-medium text-gray-700">
                        E-mail do Cliente
                      </label>
                      <div className="mt-1">
                        <input
                          {...register('emailCliente')}
                          type="email"
                          id="emailCliente"
                          className={cn(
                            "block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm",
                            errors.emailCliente && "border-red-300 focus:border-red-500 focus:ring-red-500"
                          )}
                          placeholder="cliente@exemplo.com"
                        />
                      </div>
                      {errors.emailCliente && (
                        <p className="mt-1 text-sm text-red-600">{errors.emailCliente.message}</p>
                      )}
                    </div>

                    {error && (
                      <div className="rounded-md bg-red-50 p-4">
                        <div className="flex">
                          <div className="flex-shrink-0">
                            <AlertCircle className="h-5 w-5 text-red-400" />
                          </div>
                          <div className="ml-3">
                            <p className="text-sm text-red-700">{error}</p>
                          </div>
                        </div>
                      </div>
                    )}

                    <div className="mt-5 sm:mt-4 sm:flex sm:flex-row-reverse">
                      <Button
                        type="submit"
                        isLoading={loading}
                        className="w-full sm:ml-3 sm:w-auto"
                      >
                        Enviar Convite
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={handleClose}
                        className="mt-3 w-full sm:mt-0 sm:w-auto"
                      >
                        Cancelar
                      </Button>
                    </div>
                  </form>
                )}

                {successData && (
                  <div className="mt-6 sm:mt-4 sm:flex sm:flex-row-reverse">
                    <Button
                      type="button"
                      onClick={handleDone}
                      className="w-full sm:w-auto"
                    >
                      Entendido
                    </Button>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
