import js from '@eslint/js'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import globals from 'globals'
import tseslint from 'typescript-eslint'

// Fronteiras do front-end: cada pasta em src/features/ é uma funcionalidade.
// Outras partes do código só podem usar o que a funcionalidade exporta no seu index.ts.
const fronteirasEntreFeatures = {
  'no-restricted-imports': [
    'error',
    {
      patterns: [
        {
          group: ['@/features/*/*', '**/features/*/*'],
          message:
            'Importe uma feature apenas pelo seu index.ts (por exemplo, "@/features/grupos"). Os arquivos internos de uma feature são privados.',
        },
        {
          group: ['../*/*', '../../*'],
          message: 'Use o alias "@/" para importar de fora da pasta atual.',
        },
      ],
    },
  ],
}

export default tseslint.config(
  { ignores: ['dist', 'node_modules'] },
  {
    files: ['**/*.{ts,tsx}'],
    extends: [js.configs.recommended, ...tseslint.configs.recommended],
    languageOptions: {
      ecmaVersion: 2023,
      globals: globals.browser,
    },
    plugins: {
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
    },
    rules: {
      ...reactHooks.configs.recommended.rules,
      'react-refresh/only-export-components': ['warn', { allowConstantExport: true }],
      ...fronteirasEntreFeatures,
    },
  },
)
