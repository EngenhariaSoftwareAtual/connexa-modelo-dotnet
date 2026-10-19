import { Link } from 'react-router'

export function PaginaNaoEncontrada() {
  return (
    <>
      <h1>Página não encontrada</h1>
      <p>
        <Link to="/">Voltar ao início</Link>
      </p>
    </>
  )
}
