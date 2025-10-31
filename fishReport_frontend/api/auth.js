import axios from "axios";

const API_URL = `${import.meta.env.API_BASE_URL}/api/authentication`;

export const login = async (username, password) => {
  try {
    const response = await axios.post(`${API_URL}/login`, {
      username,
      password,
    });

    const token = response.data.token; 
    localStorage.setItem('authToken', token);
    localStorage.setItem('username', username); 
    return { token, username };
  } catch (error) {
    console.error("Login failed:", error.response?.data || error.message);
    throw error;
  }
};
