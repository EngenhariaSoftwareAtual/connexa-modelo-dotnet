import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from '@/app/App'
import '@/estilos/global.css'

const raiz = document.getElementById('raiz')
if (!raiz) throw new Error('Elemento #raiz não encontrado em index.html')

createRoot(raiz).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
