import { ErroDaApi } from '@/api'

interface Props {
  erro: unknown
  /** Textos por código de erro da API; os demais usam a mensagem do servidor. */
  mensagens?: Record<string, string>
}

/** Mostra um erro de forma acessível: anunciado por leitores de tela assim que aparece. */
export function MensagemDeErro({ erro, mensagens = {} }: Props) {
  const texto =
    erro instanceof ErroDaApi ? (mensagens[erro.codigo] ?? erro.message) : 'Algo deu errado. Tente novamente.'
  return (
    <p role="alert" className="mensagem mensagem--erro">
      {texto}
    </p>
  )
}
