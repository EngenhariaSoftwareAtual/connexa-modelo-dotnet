import { useQuery } from '@tanstack/react-query'
import { requisitar } from '@/api'

/** Contrato de GET /api/identidade/sessao e /api/identidade/usuarios-simulados. */
export interface Sessao {
  chave: string
  id: string
  nome: string
  email: string
  simulado: boolean
}

export function useSessao() {
  return useQuery({
    queryKey: ['identidade', 'sessao'],
    queryFn: ({ signal }) => requisitar<Sessao>('/api/identidade/sessao', { sinal: signal }),
  })
}

export function useUsuariosSimulados() {
  return useQuery({
    queryKey: ['identidade', 'usuarios-simulados'],
    queryFn: ({ signal }) => requisitar<Sessao[]>('/api/identidade/usuarios-simulados', { sinal: signal }),
    staleTime: Infinity,
  })
}
