import React, { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [username, setUsername] = useState(null);

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("authToken");
      const storedUsername = localStorage.getItem("username");
      setIsAuthenticated(!!token);
      setUsername(storedUsername);
    };

    checkAuth();
    window.addEventListener("storage", checkAuth);

    return () => {
      window.removeEventListener("storage", checkAuth);
    };
  }, []);

  const login = (token, user) => {
    localStorage.setItem("authToken", token);
    localStorage.setItem("username", user);
    setIsAuthenticated(true);
    setUsername(user);
  };

  const logout = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("username");
    setIsAuthenticated(false);
    setUsername(null);
  };

  const value = {
    isAuthenticated,
    username,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
