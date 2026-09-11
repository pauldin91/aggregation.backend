import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5164,
    proxy: {
      '/auth': 'http://localhost:5146',
      '/api': 'http://localhost:5146',
    }
  }
})
