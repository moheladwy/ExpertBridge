import React, { createContext, useState, useContext, ReactNode, useCallback } from 'react';

interface AuthPromptContextType {
  isAuthPromptOpen: boolean;
  showAuthPrompt: () => void;
  hideAuthPrompt: () => void;
}

const AuthPromptContext = createContext<AuthPromptContextType | undefined>(undefined);

export const AuthPromptProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isAuthPromptOpen, setIsAuthPromptOpen] = useState(false);

  const showAuthPrompt = useCallback(() => {
    console.log("Auth Prompt triggered");
    setIsAuthPromptOpen(true);
  }, []);

  const hideAuthPrompt = useCallback(() => {
    setIsAuthPromptOpen(false);
  }, []);

  return (
    <AuthPromptContext.Provider value={{ isAuthPromptOpen, showAuthPrompt, hideAuthPrompt }}>
      {children}
    </AuthPromptContext.Provider>
  );
};

export const useAuthPrompt = (): AuthPromptContextType => {
  const context = useContext(AuthPromptContext);
  if (context === undefined) {
    throw new Error('useAuthPrompt must be used within AuthPromptProvider');
  }
  return context;
};