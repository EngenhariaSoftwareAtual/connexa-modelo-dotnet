import { useQuery } from '@tanstack/react-query'
import { requisitar } from '@/api'

/** Contrato de GET /api/situacao (módulo comum do back-end). */
export interface Situacao {
  aplicacao: 'disponivel' | 'indisponivel'
  banco: 'disponivel' | 'indisponivel'
  versao: string
}

export function useSituacao() {
  return useQuery({
    queryKey: ['situacao'],
    queryFn: ({ signal }) => requisitar<Situacao>('/api/situacao', { sinal: signal }),
  })
}
