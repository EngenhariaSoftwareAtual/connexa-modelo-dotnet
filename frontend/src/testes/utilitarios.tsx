import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render } from '@testing-library/react'
import type { ReactElement } from 'react'
import { MemoryRouter } from 'react-router'
import { vi } from 'vitest'

/** Renderiza um componente com os provedores da aplicação (consultas e rotas). */
export function renderizar(componente: ReactElement, { rota = '/' }: { rota?: string } = {}) {
  const clienteDeConsultas = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  })
  return render(
    <QueryClientProvider client={clienteDeConsultas}>
      <MemoryRouter initialEntries={[rota]}>{componente}</MemoryRouter>
    </QueryClientProvider>,
  )
}

type Resposta = { status?: number; corpo?: unknown }

/**
 * Simula o back-end: cada caminho da API devolve a resposta indicada.
 * Retorna o "espião" do fetch para conferir as requisições feitas.
 */
export function simularApi(respostas: Record<string, Resposta>) {
  return vi.spyOn(globalThis, 'fetch').mockImplementation(async (entrada) => {
    const caminho = typeof entrada === 'string' ? entrada : entrada instanceof URL ? entrada.pathname : entrada.url
    const resposta = respostas[caminho]
    if (!resposta) throw new Error(`Caminho não simulado no teste: ${caminho}`)
    return new Response(JSON.stringify(resposta.corpo ?? null), {
      status: resposta.status ?? 200,
      headers: { 'Content-Type': 'application/json' },
    })
  })
}
