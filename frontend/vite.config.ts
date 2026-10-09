import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// The proxy forwards /graphql and /api to the .NET backend, so the browser never hits CORS issues.
export default defineConfig({
  plugins: [react()],
  server: { port: 5173, proxy: { '/graphql': 'http://localhost:5050', '/api': 'http://localhost:5050' } },
});
