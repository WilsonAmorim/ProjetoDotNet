import React, { useState, useEffect, useContext, createContext, useCallback } from 'react';
import type { ReactNode, FormEvent } from 'react'; 

// URL base do seu PpeBackend (Certifique-se que o CORS está configurado para http://localhost:3000)
const API_BASE_URL = 'http://localhost:5271';
const AUTH_ENDPOINT = `${API_BASE_URL}/api/Auth/login`; 
const REFRESH_ENDPOINT = `${API_BASE_URL}/api/Auth/refresh`;
const PROFILE_ENDPOINT = `${API_BASE_URL}/api/Auth/perfil`; 

// ---------------------------------------------------------------------
// 1. Componentes SVG Inline (Substituindo lucide-react)
// ---------------------------------------------------------------------

interface IconProps extends React.SVGProps<SVGSVGElement> {
    className?: string;
}

const Icon = (path: string, displayName: string) => {
    const Component: React.FC<IconProps> = ({ className = "w-5 h-5", ...props }) => (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className={className}
            {...props}
        >
            <path d={path} />
        </svg>
    );
    Component.displayName = displayName;
    return Component;
};

const IconLogIn = Icon("M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4 M10 17l5-5-5-5 M15 12H3", "LogIn");
const IconLogOut = Icon("M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4 M16 17l5-5-5-5 M21 12H9", "LogOut");
const IconLoader2 = Icon("M21 12a9 9 0 1 1-6.219-8.56", "Loader2");
const IconZap = Icon("M13 2 3 14h9l-1 8 10-12h-9l1-8Z", "Zap");
const IconX = Icon("M18 6 6 18 M6 6l12 12", "X");
const IconUser = Icon("M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2 M12 7m-4 0a4 4 0 1 0 8 0a4 4 0 1 0-8 0", "User");
const IconCalendar = Icon("M19 4H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2 M16 2v4 M8 2v4 M3 10h18", "Calendar");

// ---------------------------------------------------------------------
// 2. Tipagem (Interfaces)
// ---------------------------------------------------------------------



interface UserProfileData {
  login: string;
  nome: string;
  email: string;
  role: string;
  refreshExpiraEm: string;
}

interface LoginResult {
  success: boolean;
  message?: string;
}

interface RefreshResult {
  success: boolean;
  token: string | null;
  message?: string;
}

interface AuthContextType {
  token: string | null;
  refreshToken: string | null;
  user: UserProfileData | null; // ✅ CORRIGIDO: Tipagem específica para perfil
  isLoggedIn: boolean;
  login: (username: string, password: string) => Promise<LoginResult>;
  logout: () => void;
  refreshAccessToken: (currentRefreshToken: string) => Promise<RefreshResult>;
  setUser: (profile: UserProfileData | null) => void; 
}

// ---------------------------------------------------------------------
// 3. Criação do Contexto de Autenticação
// ---------------------------------------------------------------------

const AuthContext = createContext<AuthContextType | null>(null);

const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth deve ser usado dentro de um AuthProvider');
    }
    return context;
};

// ---------------------------------------------------------------------
// 4. Provedor de Autenticação (State Management)
// ---------------------------------------------------------------------

interface AuthProviderProps {
    children: ReactNode;
}

const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('jwt_token') || null);
  const [refreshToken, setRefreshToken] = useState<string | null>(localStorage.getItem('refresh_token') || null);
  const [user, setUser] = useState<UserProfileData | null>(null); // ✅ CORRIGIDO: Tipagem específica

  // Função para renovar o Access Token usando o Refresh Token
  const refreshAccessToken = useCallback(async (currentRefreshToken: string): Promise<RefreshResult> => {
    console.log("Tentando renovar token...");
    try {
      const response = await fetch(REFRESH_ENDPOINT, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken: currentRefreshToken }),
      });

      if (!response.ok) {
        // Se a API retornar um erro (e.g., 401 para token inválido/expirado), trata
        return { success: false, token: null, message: "Refresh token inválido ou expirado." };
      }

      const data = await response.json();
      const newToken = data.token as string; 
      const newRefreshToken = data.refreshToken as string;

      if (newToken && newRefreshToken) {
        setToken(newToken);
        setRefreshToken(newRefreshToken);
        localStorage.setItem('jwt_token', newToken);
        localStorage.setItem('refresh_token', newRefreshToken);
        console.log("Token renovado com sucesso.");
        return { success: true, token: newToken };
      } else {
        return { success: false, token: null, message: "Dados de token incompletos na resposta da renovação." };
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Erro de rede durante a renovação.";
      console.error("Erro no Refresh Token:", errorMessage);
      return { success: false, token: null, message: errorMessage };
    }
  }, []);

  // Função de Login
  const login = async (username: string, password: string): Promise<LoginResult> => {
    try {
      const response = await fetch(AUTH_ENDPOINT, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Login: username, Senha: password }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, message: errorData.message || "Credenciais inválidas ou erro no servidor." };
      }

      const data = await response.json();
      
      const newToken = data.token as string; 
      const newRefreshToken = data.refreshToken as string;

      if (newToken && newRefreshToken) {
        setToken(newToken);
        setRefreshToken(newRefreshToken);
        
        localStorage.setItem('jwt_token', newToken);
        localStorage.setItem('refresh_token', newRefreshToken);
        
        return { success: true };
      } else {
        return { success: false, message: "Token ou Refresh Token não recebidos da API." };
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Erro de rede desconhecido.";
      console.error("Erro no Login:", errorMessage);
      return { success: false, message: errorMessage };
    }
  };

  // Função de Logout
  const logout = () => {
    setToken(null);
    setRefreshToken(null);
    setUser(null);
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('refresh_token');
  };

  // Valor do contexto
  const contextValue: AuthContextType = {
    token,
    refreshToken,
    user,
    isLoggedIn: !!token,
    login,
    logout,
    refreshAccessToken,
    setUser, // ✅ NOVO: Incluindo o setter no contexto
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

// ---------------------------------------------------------------------
// 5. Componente para buscar Perfil do Usuário
// ---------------------------------------------------------------------

const UserProfile: React.FC = () => {
    const { user, isLoggedIn, logout } = useAuth();

    const menuItems = [
        { id: 'overview', label: 'Visão Geral', icon: <IconZap /> },
        { id: 'perfil', label: 'Meu Perfil', icon: <IconUser /> },
        { id: 'agenda', label: 'Agenda', icon: <IconCalendar /> },
    ];

    return (
        <div className="bg-white p-6 rounded-xl shadow-lg border border-indigo-100">
            <div className="flex items-center justify-between mb-4">
                <div className="flex items-center space-x-3">
                    <IconZap className="w-6 h-6 text-indigo-500" />
                    <h3 className="text-xl font-bold text-gray-800">Menu</h3>
                </div>
                {isLoggedIn && user && (
                    <div className="text-sm text-gray-600">Olá, <span className="font-medium">{user.nome}</span></div>
                )}
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                {menuItems.map(item => (
                    <button
                        key={item.id}
                        onClick={() => console.log(`Navegar para: ${item.id}`)}
                        className="flex items-center p-3 bg-gray-50 border border-gray-100 rounded-lg hover:bg-indigo-50 transition"
                    >
                        {React.cloneElement(item.icon as React.ReactElement, { className: "w-5 h-5 mr-3 text-indigo-500" })}
                        <span className="font-medium text-gray-700">{item.label}</span>
                    </button>
                ))}
            </div>

            <div className="mt-6">
                <button
                    onClick={logout}
                    className="w-full flex items-center justify-center py-2 px-4 bg-red-500 text-white text-sm font-medium rounded-lg shadow hover:bg-red-600 transition"
                >
                    <IconLogOut className="w-4 h-4 mr-2" />
                    Sair
                </button>
            </div>
        </div>
    );
};


// ---------------------------------------------------------------------
// 7. Componente de Dashboard para Agrupar Conteúdo Protegido
// ---------------------------------------------------------------------

const DashboardContent: React.FC = () => (
    <div className="space-y-10">
        <UserProfile />
       
    </div>
);


// ---------------------------------------------------------------------
// 8. Componente de Login
// ---------------------------------------------------------------------

const LoginScreen: React.FC = () => {
    const { login } = useAuth();
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [message, setMessage] = useState<string>('');

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');

        const result = await login(username, password);
        
        if (!result.success) {
            setMessage(result.message || 'Falha desconhecida ao logar.');
        }
        
        setLoading(false);
    };

    return (
        <div className="w-full max-w-sm mx-auto p-6 bg-white rounded-xl shadow-2xl">
            <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">
                <IconZap className="inline w-6 h-6 mr-2 text-indigo-500" />
                Acesso Restrito
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="username">Usuário (Ex: user)</label>
                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500 transition duration-150"
                        placeholder="Nome de usuário"
                    />
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="password">Senha (Ex: password123)</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500 transition duration-150"
                        placeholder="Senha"
                    />
                </div>
                {message && (
                    <div className="text-sm text-red-600 bg-red-50 p-3 rounded-lg border border-red-200">
                        {message}
                    </div>
                )}

                <button
                    type="submit"
                    disabled={loading}
                    className="w-full flex justify-center items-center py-2 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition duration-150 disabled:opacity-50"
                >
                    {loading ? (
                        <IconLoader2 className="animate-spin w-5 h-5 mr-2" />
                    ) : (
                        <IconLogIn className="w-5 h-5 mr-2" />
                    )}
                    {loading ? "Entrando..." : "Entrar"}
                </button>
            </form>
        </div>
    );
};


// ---------------------------------------------------------------------
// 9. Componente de Layout Principal
// ---------------------------------------------------------------------

const AppLayout: React.FC = () => {
    const { isLoggedIn, logout } = useAuth();
    
    return (
        <div className="min-h-screen bg-gray-50 p-4 sm:p-8 font-inter">
            <header className="w-full max-w-6xl mx-auto flex justify-between items-center py-4 border-b border-indigo-100 mb-8">
                <h1 className="text-3xl font-extrabold text-indigo-700">PpeFrontend React (TS)</h1>
                {isLoggedIn && (
                    <button
                        onClick={logout}
                        className="flex items-center px-4 py-2 bg-red-500 text-white text-sm font-medium rounded-lg shadow hover:bg-red-600 transition duration-150"
                    >
                        <IconLogOut className="w-4 h-4 mr-2" />
                        Sair
                    </button>
                )}
            </header>

            <main className="w-full max-w-6xl mx-auto">
                {!isLoggedIn ? (
                    <div className="flex justify-center items-center py-20">
                        <LoginScreen />
                    </div>
                ) : (
                    <DashboardContent /> 
                )}
            </main>
            
            <footer className="w-full max-w-6xl mx-auto text-center mt-12 pt-6 border-t text-sm text-gray-400">
                <p>Status da API: {API_BASE_URL}</p>
                <p>Para teste, use as credenciais padrão no componente de login.</p>
            </footer>
        </div>
    );
};

// ---------------------------------------------------------------------
// 10. Componente Raiz
// ---------------------------------------------------------------------

const App: React.FC = () => (
    <AuthProvider>
        <AppLayout />
    </AuthProvider>
);

export default App;