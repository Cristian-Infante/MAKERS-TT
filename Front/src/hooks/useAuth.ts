"use client";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/apiClient";
import { saveSession, clearSession, getToken } from "@/lib/storage";
import { isTokenExpired } from "@/lib/auth";
import { APP_ROUTES } from "@/constants/routes";
import type { AuthResponse } from "@/lib/types";

export function useAuth() {
  const router = useRouter();

  async function login(email: string, password: string): Promise<void> {
    const data = await apiClient.post<AuthResponse>(
      "/api/v1/Auth/login",
      { email, password }
    );
    saveSession(data.token, data.role);
    router.push(data.role === "Admin" ? APP_ROUTES.ADMIN : APP_ROUTES.USER);
  }

  async function register(
    email: string,
    password: string,
    fullName: string
  ): Promise<void> {
    const data = await apiClient.post<AuthResponse>(
      "/api/v1/Auth/register",
      { email, password, fullName }
    );
    saveSession(data.token, data.role);
    router.push(APP_ROUTES.USER);
  }

  function logout(): void {
    clearSession();
    router.push(APP_ROUTES.LOGIN);
  }

  function checkSession(): void {
    const token = getToken();
    if (token && isTokenExpired(token)) {
      clearSession();
      router.push(APP_ROUTES.LOGIN);
    }
  }

  return { login, register, logout, checkSession };
}
