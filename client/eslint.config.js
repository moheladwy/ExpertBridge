import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'

export default tseslint.config(
  { ignores: ['dist', 'node_modules'] }, // Ignore more folders
  {
    extends: [
      js.configs.recommended,
      ...tseslint.configs.recommended,
    ],
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    plugins: {
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
    },
    rules: {
      ...reactHooks.configs.recommended.rules,

      // Less restrictive rules
      'react-refresh/only-export-components': 'off', // Allow exports anywhere
      '@typescript-eslint/no-explicit-any': 'off', // Allow `any` type
      '@typescript-eslint/explicit-module-boundary-types': 'off', // No need to define return types explicitly
      "@typescript-eslint/no-unused-vars": ["warn", { argsIgnorePattern: "^_", varsIgnorePattern: "^_" }],
      'no-console': 'off', // Allow console logs
      'no-debugger': 'warn', // Warn instead of error for debugger
      'react-hooks/rules-of-hooks': 'warn', // Allow some flexibility in hooks usage
      'react-hooks/exhaustive-deps': 'warn', // Don't enforce strict dependency checks
    },
  },
)
