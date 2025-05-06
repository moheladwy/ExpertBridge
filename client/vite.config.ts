import { defineConfig } from 'vite'
import path from "path"
import react from '@vitejs/plugin-react-swc'

export default defineConfig({
  plugins: [react()],
  resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
        "@assets": path.resolve(__dirname, "./src/assets"),
        "@views": path.resolve(__dirname, "./src/views"),
      },
    },
})
