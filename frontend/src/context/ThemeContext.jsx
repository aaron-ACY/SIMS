import React, { createContext, useContext, useState, useEffect } from 'react';

const ThemeContext = createContext();

export const themes = {
  light: {
    name: 'Sáng',
    colors: {
      primary: '#aa3bff',
      primaryDark: '#8a2be2',
      bg: '#ffffff',
      sidebarBg: '#ffffff',
      text: '#000000',
      textMuted: '#4b5563',
      hover: '#f3e8ff',
      border: '#f3f4f6',
    }
  },
  dark: {
    name: 'Tối',
    colors: {
      primary: '#aa3bff',
      primaryDark: '#8a2be2',
      bg: '#0f172a',
      sidebarBg: '#1e293b',
      text: '#ffffff',
      textMuted: '#94a3b8',
      hover: '#334155',
      border: '#334155',
    }
  },
  jungle: {
    name: 'Jungle',
    colors: {
      primary: '#2FA084',
      primaryDark: '#1F6F5F',
      bg: '#EEEEEE',
      sidebarBg: '#ffffff',
      text: '#0a2f27',
      textMuted: '#1F6F5F',
      hover: '#dcfce7',
      border: '#e5e7eb',
    }
  },
  spring: {
    name: 'Spring',
    colors: {
      primary: '#d484a0',
      primaryDark: '#a64d6a',
      bg: '#F9F5F6',
      sidebarBg: '#ffffff',
      text: '#5a1a2e',
      textMuted: '#a64d6a',
      hover: '#FDCEDF',
      border: '#F8E8EE',
    }
  }
};

export const ThemeProvider = ({ children }) => {
  const [currentTheme, setCurrentTheme] = useState(() => {
    return localStorage.getItem('sims-theme') || 'jungle';
  });

  useEffect(() => {
    const themeData = themes[currentTheme];
    const root = document.documentElement;

    Object.entries(themeData.colors).forEach(([key, value]) => {
      root.style.setProperty(`--theme-${key}`, value);
    });

    localStorage.setItem('sims-theme', currentTheme);
  }, [currentTheme]);

  return (
    <ThemeContext.Provider value={{ currentTheme, setCurrentTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = () => useContext(ThemeContext);
