import { useQueryClient } from '@tanstack/react-query'
import { guardarUsuarioSimulado } from '@/api'
import { MensagemDeErro } from '@/compartilhado'
import { useSessao, useUsuariosSimulados } from './consultas'

/**
 * Enquanto não há autenticação real, permite escolher qual estudante simulado
 * está usando o sistema. Trocar o estudante recarrega todos os dados da tela.
 */
export function SeletorDeEstudante() {
  const clienteDeConsultas = useQueryClient()
  const sessao = useSessao()
  const usuarios = useUsuariosSimulados()

  if (sessao.error) {
    return <MensagemDeErro erro={sessao.error} mensagens={{ USUARIO_SIMULADO_INEXISTENTE: 'Escolha um estudante simulado válido.' }} />
  }

  function trocar(chave: string) {
    guardarUsuarioSimulado(chave)
    void clienteDeConsultas.invalidateQueries()
  }

  return (
    <div className="seletor-de-estudante">
      <label htmlFor="estudante-simulado">Estudante (simulado)</label>
      <select
        id="estudante-simulado"
        value={sessao.data?.chave ?? ''}
        disabled={!sessao.data || !usuarios.data}
        onChange={(evento) => trocar(evento.target.value)}
      >
        {!sessao.data && <option value="">Carregando…</option>}
        {usuarios.data?.map((usuario) => (
          <option key={usuario.chave} value={usuario.chave}>
            {usuario.nome}
          </option>
        ))}
      </select>
    </div>
  )
}
