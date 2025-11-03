// Server URL configuration
const SERVER_BASE_URL =
  import.meta.env.VITE_SERVER_URL || "http://localhost:5000";

// API endpoint with /api path
const VITE_SERVER_URL = `${SERVER_BASE_URL}/api`;

// Optional HTTPS API URL
const API_HTTPS_BASE_URL = import.meta.env.VITE_API_HTTPS_BASE_URL;

const config = {
  API_HTTPS_BASE_URL,
  VITE_SERVER_URL,
};

export default config;
