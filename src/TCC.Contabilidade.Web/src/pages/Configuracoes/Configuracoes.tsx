import React, { useState, useEffect } from 'react';
import { Settings, Save, AlertCircle, CheckCircle2, Building2 } from 'lucide-react';
import { Button } from '../../components/Button';
import { Input } from '../../components/Input';
import { configuracoesService } from '../../services/configuracoesService';
import { empresaService } from '../../services/empresaService';
import type { CompanyConfig } from '../../interfaces/configuracoes';
import type { Empresa } from '../../interfaces/empresa';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

const configSchema = z.object({
  moedaPadrao: z.string().min(1, 'Moeda é obrigatória'),
  formatoData: z.string().min(1, 'Formato de data é obrigatório'),
  timezone: z.string().min(1, 'Timezone é obrigatória'),
  notificacoesEmail: z.boolean(),
});

type ConfigFormData = z.infer<typeof configSchema>;

export const Configuracoes: React.FC = () => {
  const [empresas, setEmpresas] = useState<Empresa[]>([]);
  const [selectedEmpresaId, setSelectedEmpresaId] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [loadingEmpresas, setLoadingEmpresas] = useState(true);
  const [message, setMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ConfigFormData>({
    resolver: zodResolver(configSchema),
  });

  useEffect(() => {
    async function loadEmpresas() {
      try {
        const response = await empresaService.getAll(1, 100);
        if (response.sucesso && response.dados) {
          setEmpresas(response.dados);
          if (response.dados.length > 0) {
            setSelectedEmpresaId(response.dados[0].id);
          }
        }
      } catch (error) {
        console.error('Erro ao carregar empresas:', error);
      } finally {
        setLoadingEmpresas(false);
      }
    }
    loadEmpresas();
  }, []);

  useEffect(() => {
    if (selectedEmpresaId) {
      loadConfiguracoes(selectedEmpresaId);
    }
  }, [selectedEmpresaId]);

  const loadConfiguracoes = async (id: string) => {
    setLoading(true);
    setMessage(null);
    try {
      const response = await configuracoesService.getConfiguracoes(id);
      if (response.sucesso && response.dados) {
        reset({
          moedaPadrao: response.dados.moedaPadrao,
          formatoData: response.dados.formatoData,
          timezone: response.dados.timezone,
          notificacoesEmail: response.dados.notificacoesEmail,
        });
      }
    } catch (error) {
      console.error('Erro ao carregar configurações:', error);
      setMessage({ type: 'error', text: 'Erro ao carregar configurações da empresa.' });
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: ConfigFormData) => {
    if (!selectedEmpresaId) return;

    setLoading(true);
    setMessage(null);
    try {
      const payload: CompanyConfig = {
        ...data,
        empresaId: selectedEmpresaId,
      };
      await configuracoesService.updateConfiguracoes(selectedEmpresaId, payload);
      setMessage({ type: 'success', text: 'Configurações atualizadas com sucesso!' });
    } catch (error: any) {
      setMessage({
        type: 'error',
        text: error.response?.data?.mensagem || 'Erro ao atualizar configurações.'
      });
    } finally {
      setLoading(false);
    }
  };

  if (loadingEmpresas) {
    return <div className="flex justify-center py-12">Carregando...</div>;
  }

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Configurações da Empresa</h1>
        <p className="text-gray-600">Ajuste as preferências globais para a empresa selecionada.</p>
      </div>

      <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
        <div className="flex items-center space-x-2 mb-6">
          <Building2 className="h-5 w-5 text-blue-600" />
          <h2 className="text-lg font-semibold text-gray-800">Seleção de Empresa</h2>
        </div>

        <div className="max-w-xs">
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Empresa Ativa
          </label>
          <select
            value={selectedEmpresaId}
            onChange={(e) => setSelectedEmpresaId(e.target.value)}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm h-10 border px-3"
          >
            {empresas.map((empresa) => (
              <option key={empresa.id} value={empresa.id}>
                {empresa.nomeFantasia}
              </option>
            ))}
          </select>
          {empresas.length === 0 && (
            <p className="mt-2 text-sm text-amber-600">Nenhuma empresa encontrada.</p>
          )}
        </div>
      </div>

      {selectedEmpresaId && (
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center space-x-2 mb-6">
            <Settings className="h-5 w-5 text-blue-600" />
            <h2 className="text-lg font-semibold text-gray-800">Preferências do Sistema</h2>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Input
                label="Moeda Padrão"
                placeholder="Ex: BRL"
                {...register('moedaPadrao')}
                error={errors.moedaPadrao?.message}
              />
              <Input
                label="Formato de Data"
                placeholder="Ex: dd/MM/yyyy"
                {...register('formatoData')}
                error={errors.formatoData?.message}
              />
              <Input
                label="Timezone"
                placeholder="Ex: America/Sao_Paulo"
                {...register('timezone')}
                error={errors.timezone?.message}
              />

              <div className="flex items-center space-x-2 pt-8">
                <input
                  type="checkbox"
                  id="notificacoesEmail"
                  {...register('notificacoesEmail')}
                  className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <label htmlFor="notificacoesEmail" className="text-sm font-medium text-gray-700">
                  Habilitar Notificações por E-mail
                </label>
              </div>
            </div>

            {message && (
              <div className={`p-3 rounded-md flex items-center space-x-2 text-sm ${
                message.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'
              }`}>
                {message.type === 'success' ? <CheckCircle2 className="h-4 w-4" /> : <AlertCircle className="h-4 w-4" />}
                <span>{message.text}</span>
              </div>
            )}

            <div className="flex justify-end pt-4">
              <Button type="submit" isLoading={loading}>
                <Save className="h-4 w-4 mr-2" />
                Salvar Configurações
              </Button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
};
