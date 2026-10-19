/**
 * Erro devolvido pela API do Connexa no formato Problem Detail (RFC 9457).
 *
 * Use `codigo` para decidir o que mostrar ao usuário — nunca compare o texto
 * da mensagem, que pode mudar.
 */
export class ErroDaApi extends Error {
  readonly status: number
  readonly codigo: string
  readonly campos: Record<string, string>

  constructor(status: number, codigo: string, mensagem: string, campos: Record<string, string> = {}) {
    super(mensagem)
    this.name = 'ErroDaApi'
    this.status = status
    this.codigo = codigo
    this.campos = campos
  }
}

/** Código usado quando a resposta de erro não traz um `codigo` (falha de rede, erro inesperado). */
export const ERRO_INESPERADO = 'ERRO_INESPERADO'
