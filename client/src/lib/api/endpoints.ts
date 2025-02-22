export const BASE_URL = import.meta.env.VITE_SERVER_URL;
export const HEALTH_CHECK_URL = `${BASE_URL}/health`;
export const API_URL = `${BASE_URL}/api`;
export const USER_URL = `${API_URL}/User`;
export const MEDIA_URL = `${API_URL}/Media`;

export const USER_ENDPOINTS = {
  GET_USER_BY_FIREBASE_ID: `${USER_URL}/get`,
  REGISTER_NEW_USER: `${USER_URL}/register`,
}

export const MEDIA_ENDPOINTS = {
  DOWNLOAD_MEDIA_BY_KEY: `${MEDIA_URL}/download`,
  GET_MEDIA_URL_BY_KEY: `${MEDIA_URL}/url`,
  GET_PRESIGNED_URL_BY_KEY: `${MEDIA_URL}/presigned-url`,
  UPLOAD_MEDIA: `${MEDIA_URL}/upload`,
  DELETE_MEDIA_BY_KEY: `${MEDIA_URL}/delete`,
}