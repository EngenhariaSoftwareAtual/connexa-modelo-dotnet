import { Route, Routes } from 'react-router'
import { Layout } from './Layout'
import { PaginaInicial } from './PaginaInicial'
import { PaginaNaoEncontrada } from './PaginaNaoEncontrada'

/**
 * Rotas da aplicação. Cada funcionalidade nova ganha a sua rota aqui,
 * apontando para um componente exportado pelo index.ts da feature.
 */
export function Rotas() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<PaginaInicial />} />
        <Route path="*" element={<PaginaNaoEncontrada />} />
      </Route>
    </Routes>
  )
}
