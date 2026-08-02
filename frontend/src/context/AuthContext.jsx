import React, { createContext, useState, useContext } from 'react';
import { authService } from '../api/services';

const AuthContext = createContext();

// Helper to decode JWT payload claims
const parseJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('userInfo');
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const login = async (username, password) => {
    try {
      const response = await authService.login(username, password);
      if (response && response.success && response.result && response.result.accessToken) {
        const token = response.result.accessToken;
        localStorage.setItem('access_token', token);
        
        // Decode JWT claims to extract role
        // For standard .NET Identity, roles are usually in a specific claim URI.
        // We will try standard 'role' or the .NET claim schema for roles.
        const claims = parseJwt(token);
        const roleClaim = claims?.role || claims?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Admin';

        let redirectPath = '/admin';
        let formattedRole = 'ADMIN';

        if (typeof roleClaim === 'string') {
          if (roleClaim.toLowerCase().includes('instructor') || roleClaim.toLowerCase().includes('lecturer')) {
            redirectPath = '/lecturer';
            formattedRole = 'LECTURER';
          } else if (roleClaim.toLowerCase().includes('student')) {
            redirectPath = '/student';
            formattedRole = 'STUDENT';
          }
        }

        const userObj = {
          username: username,
          role: formattedRole,
          name: username,
          token: token,
        };

        setUser(userObj);
        localStorage.setItem('userInfo', JSON.stringify(userObj));
        return { success: true, redirect: redirectPath };
      }
      return { success: false, message: response?.message || 'Login failed' };
    } catch (apiError) {
      console.error('Backend API login failed:', apiError);
      return { success: false, message: apiError?.response?.data?.message || 'Invalid username or password!' };
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (e) {
      // Ignore API logout error if offline
    }
    localStorage.removeItem('access_token');
    localStorage.removeItem('userInfo');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);