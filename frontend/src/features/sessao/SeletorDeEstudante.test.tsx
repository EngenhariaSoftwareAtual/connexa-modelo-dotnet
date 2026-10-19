import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { CABECALHO_USUARIO_SIMULADO } from '@/api'
import { renderizar, simularApi } from '@/testes/utilitarios'
import { SeletorDeEstudante } from './SeletorDeEstudante'

const alice = { chave: 'alice', id: 'a', nome: 'Alice Prado', email: 'alice@instituicao.example', simulado: true }
const bento = { chave: 'bento', id: 'b', nome: 'Bento Ramos', email: 'bento@instituicao.example', simulado: true }

describe('Funcionalidade: sessão do estudante simulado (tela)', () => {
  it('Cenário: sem indicação, a sessão é do primeiro estudante simulado', async () => {
    simularApi({
      '/api/identidade/sessao': { corpo: alice },
      '/api/identidade/usuarios-simulados': { corpo: [alice, bento] },
    })

    renderizar(<SeletorDeEstudante />)

    const seletor = await screen.findByLabelText('Estudante (simulado)')
    await waitFor(() => expect(seletor).toHaveValue('alice'))
  })

  it('Cenário: estudante simulado escolhido pelo usuário', async () => {
    const api = simularApi({
      '/api/identidade/sessao': { corpo: alice },
      '/api/identidade/usuarios-simulados': { corpo: [alice, bento] },
    })
    renderizar(<SeletorDeEstudante />)
    const seletor = await screen.findByLabelText('Estudante (simulado)')
    await waitFor(() => expect(seletor).toBeEnabled())

    await userEvent.selectOptions(seletor, 'bento')

    await waitFor(() => {
      const ultimaChamada = api.mock.calls.at(-1)
      const cabecalhos = ultimaChamada?.[1]?.headers as Record<string, string> | undefined
      expect(cabecalhos?.[CABECALHO_USUARIO_SIMULADO]).toBe('bento')
    })
  })
})
