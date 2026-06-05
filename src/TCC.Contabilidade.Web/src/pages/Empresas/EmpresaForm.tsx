import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { empresaService } from '../../services/empresaService';
import { cnpjService } from '../../services/cnpjService';
import { Input } from '../../components/Input';
import { Button } from '../../components/Button';
import { ArrowLeft, Search, Save, AlertCircle } from 'lucide-react';

const empresaSchema = z.object({
  nome: z.string().min(3, 'O nome deve ter pelo menos 3 caracteres'),
  cnpj: z.string().length(14, 'O CNPJ deve ter 14 dígitos (somente números)'),
});

type EmpresaFormData = z.infer<typeof empresaSchema>;

export const EmpresaForm = () => {
  const { id } = useParams<{ id: string }>();
  const isEditing = !!id;
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [consultingCnpj, setConsultingCnpj] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<EmpresaFormData>({
    resolver: zodResolver(empresaSchema),
    defaultValues: {
      nome: '',
      cnpj: '',
    },
  });

  const cnpjValue = watch('cnpj');

  useEffect(() => {
    if (isEditing) {
      const fetchEmpresa = async () => {
        try {
          setLoading(true);
          const response = await empresaService.getById(id);
          if (response.sucesso) {
            const empresa = response.dados;
            setValue('nome', empresa.nomeFantasia);
            setValue('cnpj', empresa.cnpj);
          } else {
            setError(response.mensagem);
          }
        } catch (err) {
          setError('Erro ao carregar dados da empresa.');
          console.error(err);
        } finally {
          setLoading(false);
        }
      };
      fetchEmpresa();
    }
  }, [id, isEditing, setValue]);

  const handleConsultCnpj = async () => {
    if (cnpjValue?.length !== 14) return;

    try {
      setConsultingCnpj(true);
      setError(null);
      const response = await cnpjService.consultar(cnpjValue);
      if (response.sucesso) {
        setValue('nome', response.dados.nomeFantasia || response.dados.razaoSocial);
      } else {
        setError(response.mensagem);
      }
    } catch (err) {
      setError('Erro ao consultar CNPJ. Verifique se o número está correto.');
      console.error(err);
    } finally {
      setConsultingCnpj(false);
    }
  };

  const onSubmit = async (data: EmpresaFormData) => {
    try {
      setLoading(true);
      setError(null);
      const response = isEditing
        ? await empresaService.update(id, data)
        : await empresaService.create(data);

      if (response.sucesso) {
        navigate('/empresas');
      } else {
        setError(response.mensagem);
      }
    } catch (err: unknown) {
      const errorMessage = (err as { response?: { data?: { mensagem?: string } } }).response?.data?.mensagem || 'Erro ao salvar empresa.';
      setError(errorMessage);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center space-x-4">
        <Button
          variant="outline"
          size="sm"
          onClick={() => navigate('/empresas')}
          className="flex items-center space-x-1"
        >
          <ArrowLeft className="h-4 w-4" />
          <span>Voltar</span>
        </Button>
        <h1 className="text-2xl font-bold text-gray-900">
          {isEditing ? 'Editar Empresa' : 'Nova Empresa'}
        </h1>
      </div>

      <div className="bg-white shadow rounded-lg p-6">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded relative flex items-center space-x-2">
              <AlertCircle className="h-5 w-5" />
              <span>{error}</span>
            </div>
          )}

          <div className="flex gap-2 items-end">
            <div className="flex-1">
              <Input
                label="CNPJ (Somente números)"
                placeholder="Ex: 00000000000100"
                maxLength={14}
                {...register('cnpj')}
                error={errors.cnpj?.message}
              />
            </div>
            <Button
              type="button"
              variant="outline"
              onClick={handleConsultCnpj}
              disabled={cnpjValue?.length !== 14 || consultingCnpj}
              isLoading={consultingCnpj}
              className="mb-[2px]"
            >
              <Search className="h-4 w-4 mr-2" />
              Consultar
            </Button>
          </div>

          <Input
            label="Nome / Razão Social"
            placeholder="Ex: Minha Empresa LTDA"
            {...register('nome')}
            error={errors.nome?.message}
          />

          <div className="flex justify-end pt-4">
            <Button
              type="button"
              variant="outline"
              className="mr-3"
              onClick={() => navigate('/empresas')}
            >
              Cancelar
            </Button>
            <Button type="submit" isLoading={loading}>
              <Save className="h-4 w-4 mr-2" />
              {isEditing ? 'Salvar Alterações' : 'Cadastrar Empresa'}
            </Button>
          </div>
        </form>
      </div>

      {!isEditing && (
        <div className="bg-blue-50 border-l-4 border-blue-400 p-4 rounded">
          <div className="flex">
            <div className="flex-shrink-0">
              <AlertCircle className="h-5 w-5 text-blue-400" />
            </div>
            <div className="ml-3">
              <p className="text-sm text-blue-700">
                Ao cadastrar uma nova empresa, você será automaticamente vinculado a ela como contador responsável.
              </p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
