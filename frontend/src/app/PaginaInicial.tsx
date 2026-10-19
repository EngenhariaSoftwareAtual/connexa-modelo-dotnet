import { PainelSituacao } from '@/features/situacao'

export function PaginaInicial() {
  return (
    <>
      <h1>Grupos de estudo, sem complicação</h1>
      <p className="introducao">
        Ambiente de desenvolvimento do Connexa. As funcionalidades aparecem aqui à medida que as histórias da sprint
        são concluídas.
      </p>
      <PainelSituacao />
    </>
  )
}
