import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

// ESTE IMPORT É CRÍTICO para o Tailwind CSS funcionar!
// Ele injeta as classes base, componentes e utilitários na aplicação.
import './index.css'; 

// Localiza o elemento root no HTML e renderiza a aplicação
ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);