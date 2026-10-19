import { ERRO_INESPERADO, ErroDaApi } from './ErroDaApi'

/**
 * Cliente HTTP do Connexa. Toda chamada ao back-end passa por aqui.
 *
 * - Envia o estudante simulado escolhido (enquanto não há autenticação real).
 * - Converte respostas de erro em `ErroDaApi`, com `status`, `codigo` e `campos`.
 */

export const CABECALHO_USUARIO_SIMULADO = 'X-Connexa-Usuario-Simulado'
const CHAVE_ARMAZENADA = 'connexa.usuarioSimulado'

export function lerUsuarioSimulado(): string | null {
  try {
    return window.localStorage.getItem(CHAVE_ARMAZENADA)
  } catch {
    return null
  }
}

export function guardarUsuarioSimulado(chave: string): void {
  try {
    window.localStorage.setItem(CHAVE_ARMAZENADA, chave)
  } catch {
    // Sem armazenamento disponível: vale o estudante padrão do back-end.
  }
}

type Corpo = Record<string, unknown> | unknown[]

interface Opcoes {
  metodo?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'
  corpo?: Corpo
  sinal?: AbortSignal
}

export async function requisitar<T>(caminho: string, opcoes: Opcoes = {}): Promise<T> {
  const cabecalhos: Record<string, string> = { Accept: 'application/json' }
  const usuario = lerUsuarioSimulado()
  if (usuario) cabecalhos[CABECALHO_USUARIO_SIMULADO] = usuario
  if (opcoes.corpo !== undefined) cabecalhos['Content-Type'] = 'application/json'

  let resposta: Response
  try {
    resposta = await fetch(caminho, {
      method: opcoes.metodo ?? 'GET',
      headers: cabecalhos,
      body: opcoes.corpo === undefined ? undefined : JSON.stringify(opcoes.corpo),
      signal: opcoes.sinal,
    })
  } catch {
    throw new ErroDaApi(0, ERRO_INESPERADO, 'Não foi possível falar com o servidor.')
  }

  if (resposta.status === 204) return undefined as T

  const conteudo: unknown = await resposta.json().catch(() => null)

  if (!resposta.ok) {
    const problema = (conteudo ?? {}) as { codigo?: string; detail?: string; campos?: Record<string, string> }
    throw new ErroDaApi(
      resposta.status,
      problema.codigo ?? ERRO_INESPERADO,
      problema.detail ?? `O servidor respondeu com erro ${resposta.status}.`,
      problema.campos ?? {},
    )
  }

  return conteudo as T
}
