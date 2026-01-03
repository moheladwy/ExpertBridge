// Base server URL from environment or default
export const BASE_URL =
	process.env.NEXT_PUBLIC_SERVER_URL || "http://localhost:5027";
export const API_URL = `${BASE_URL}/api`;
export const SEARCH_URL = `${API_URL}/Search`;

export const SEARCH_ENDPOINTS = {
	SEARCH_USERS: `${SEARCH_URL}/users`,
	SEARCH_POSTS: `${SEARCH_URL}/posts`,
	SEARCH_JOB_POSTS: `${SEARCH_URL}/jobs`,
};
