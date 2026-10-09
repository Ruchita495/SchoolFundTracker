import { defineConfig } from '@playwright/test';
// Start the backend (port 5000) and `npm run dev` (port 5173) before running `npm run e2e`.
export default defineConfig({ testDir: './e2e', use: { baseURL: 'http://localhost:5173' } });
