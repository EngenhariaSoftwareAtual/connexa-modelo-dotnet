import { screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { renderizar, simularApi } from '@/testes/utilitarios'
import { PainelSituacao } from './PainelSituacao'

describe('Funcionalidade: situação do sistema (tela)', () => {
  it('Cenário: aplicação e banco disponíveis', async () => {
    simularApi({
      '/api/situacao': { corpo: { aplicacao: 'disponivel', banco: 'disponivel', versao: '0.1.0' } },
    })

    renderizar(<PainelSituacao />)

    expect(await screen.findByText('0.1.0')).toBeInTheDocument()
    expect(screen.getAllByText('Disponível')).toHaveLength(2)
  })

  it('Cenário: banco de dados indisponível', async () => {
    simularApi({
      '/api/situacao': { status: 503, corpo: { aplicacao: 'disponivel', banco: 'indisponivel', versao: '0.1.0' } },
    })

    renderizar(<PainelSituacao />)

    expect(await screen.findByRole('alert')).toHaveTextContent('O banco de dados está indisponível.')
  })
})
