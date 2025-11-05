/** @type {import('tailwindcss').Config} */
export default {
  // Lista de arquivos que o Tailwind deve escanear para detectar classes que estão sendo usadas.
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      // Aqui você pode adicionar cores customizadas, tamanhos, fontes, etc.
      // Por exemplo, definindo a fonte 'Inter' como padrão:
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [
    // Plugins adicionais do Tailwind, como formulários ou tipografia
  ],
}