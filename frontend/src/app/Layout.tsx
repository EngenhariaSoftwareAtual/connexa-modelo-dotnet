import { Link, Outlet } from 'react-router'
import { SeletorDeEstudante } from '@/features/sessao'

export function Layout() {
  return (
    <>
      <a className="pular-para-conteudo" href="#conteudo">
        Pular para o conteúdo
      </a>
      <header className="cabecalho">
        <Link to="/" className="marca">
          Connexa
        </Link>
        <SeletorDeEstudante />
      </header>
      <main id="conteudo" className="conteudo" tabIndex={-1}>
        <Outlet />
      </main>
    </>
  )
}
