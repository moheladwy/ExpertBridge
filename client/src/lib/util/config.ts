
const API_HTTPS_BASE_URL = import.meta.env.VITE_API_HTTPS_BASE_URL;
const VITE_SERVER_URL =
  import.meta.env.REACT_APP_ENV == 'DEV' 
  ? import.meta.env.VITE_LOCAL_API_HTTP_BASE_URL
  : import.meta.env.VITE_SERVER_URL;

const config = {
  API_HTTPS_BASE_URL,
  VITE_SERVER_URL,
};

export default config;
