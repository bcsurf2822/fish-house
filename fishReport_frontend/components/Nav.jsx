import React, { useState } from "react";
import { Link } from "react-router";
import { login } from "../api/auth";
import { useAuth } from "../contexts/authContext";
import toast from "react-hot-toast";

const Nav = () => {
  const {
    isAuthenticated,
    username,
    login: authLogin,
    logout: authLogout,
  } = useAuth();
  const [credentials, setCredentials] = useState({
    username: "",
    password: "",
  });

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const { token, username: loggedInUsername } = await login(
        credentials.username,
        credentials.password
      );
      authLogin(token, loggedInUsername);
      setCredentials({ username: "", password: "" });
      toast.success("Successfully logged in!");
    } catch (error) {
      const errorMessage =
        error.response?.data || "Login failed. Please check your credentials.";
      toast.error(errorMessage);
      console.error("Login failed:", error);
    }
  };

  const handleLogout = () => {
    authLogout();
    toast.success("Successfully logged out!");
  };

  return (
    <nav className="flex justify-between items-center px-8 py-4 bg-white border-b border-gray-200 shadow-sm">
      <div>
        <h2 className="text-2xl font-semibold bg-gradient-to-r from-teal-600 to-cyan-600 bg-clip-text text-transparent">
          Fish Inventory Application
        </h2>
      </div>

      <div className="flex gap-8">
        <Link
          to="/"
          className="text-gray-700 hover:text-teal-600 font-medium transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-teal-50"
        >
          Markets
        </Link>
        <Link
          to="/fish"
          className="text-gray-700 hover:text-teal-600 font-medium transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-teal-50"
        >
          Fish
        </Link>
        <Link
          to="/instructions"
          className="text-gray-700 hover:text-teal-600 font-medium transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-teal-50"
        >
          Instructions
        </Link>
      </div>

      <div className="flex gap-3">
        {!isAuthenticated ? (
          <>
            <input
              type="text"
              placeholder="Username"
              value={credentials.username}
              onChange={(e) =>
                setCredentials({ ...credentials, username: e.target.value })
              }
              className="px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all duration-200 shadow-sm"
            />
            <input
              type="password"
              placeholder="Password"
              value={credentials.password}
              onChange={(e) =>
                setCredentials({ ...credentials, password: e.target.value })
              }
              className="px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all duration-200 shadow-sm"
            />
            <button
              onClick={handleLogin}
              className="px-6 py-2 bg-gradient-to-r from-teal-600 to-cyan-600 text-white rounded-lg hover:from-teal-700 hover:to-cyan-700 transition-all duration-200 shadow-sm hover:shadow-md font-medium"
            >
              Login
            </button>
          </>
        ) : (
          <div className="flex items-center gap-4">
            <span className="text-gray-700 font-medium">Welcome, <span className="text-teal-600 font-semibold">{username}</span>!</span>
            <button
              onClick={handleLogout}
              className="px-5 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-all duration-200 shadow-sm hover:shadow-md font-medium"
            >
              Logout
            </button>
          </div>
        )}
      </div>
    </nav>
  );
};

export default Nav;
