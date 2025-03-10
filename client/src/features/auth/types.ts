import { User } from "firebase/auth";



export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: { id: number; name: string };
}

export interface RegisterRequest {
  email: string;
  password: string;
}
export interface RegisterResponse {
  user: User;
}
