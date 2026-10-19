import { ErroDaApi } from '@/api'
import { MensagemDeErro } from '@/compartilhado'
import { useSituacao, type Situacao } from './consultas'

/**
 * Fatia de exemplo do modelo: mostra se o back-end e o banco estão respondendo.
 * Não é uma funcionalidade do Connexa — serve de referência de estrutura.
 */
export function PainelSituacao() {
  const { data, error, isPending } = useSituacao()

  return (
    <section className="cartao" aria-labelledby="titulo-situacao">
      <h2 id="titulo-situacao">Situação do sistema</h2>

      {isPending && <p aria-live="polite">Consultando o servidor…</p>}

      {error && (
        <MensagemDeErro
          erro={error}
          mensagens={{ ERRO_INESPERADO: textoDoErro(error) }}
        />
      )}

      {data && (
        <dl className="lista-de-dados">
          <div>
            <dt>Aplicação</dt>
            <dd>{rotulo(data.aplicacao)}</dd>
          </div>
          <div>
            <dt>Banco de dados</dt>
            <dd>{rotulo(data.banco)}</dd>
          </div>
          <div>
            <dt>Versão</dt>
            <dd>{data.versao}</dd>
          </div>
        </dl>
      )}
    </section>
  )
}

function rotulo(valor: Situacao['banco']) {
  return valor === 'disponivel' ? 'Disponível' : 'Indisponível'
}

function textoDoErro(erro: unknown) {
  if (erro instanceof ErroDaApi && erro.status === 503) return 'O banco de dados está indisponível.'
  if (erro instanceof ErroDaApi && erro.status === 0) return 'O back-end não está respondendo. Ele está em execução?'
  return 'Não foi possível consultar a situação do sistema.'
}
