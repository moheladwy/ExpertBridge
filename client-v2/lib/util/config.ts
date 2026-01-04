/**
 * Application configuration.
 *
 * All environment variables must be prefixed with NEXT_PUBLIC_ to be
 * accessible in client components.
 */
export const config = {
  /** Backend API base URL */
  serverUrl: process.env.NEXT_PUBLIC_SERVER_URL ?? "http://localhost:5027",

  /** API endpoints prefix */
  apiPrefix: "/api",

  /** Full API base URL */
  get apiUrl(): string {
    return `${this.serverUrl}${this.apiPrefix}`;
  },

  /** Enable debug logging */
  isDebugEnabled: process.env.NEXT_PUBLIC_ENABLE_DEBUG_LOGGING === "true",

  /** Current environment */
  isDevelopment: process.env.NODE_ENV === "development",
  isProduction: process.env.NODE_ENV === "production",
} as const;
