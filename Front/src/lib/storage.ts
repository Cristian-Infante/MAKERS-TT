import { COOKIE_TOKEN, COOKIE_ROLE } from "@/constants/cookies";

export function saveSession(token: string, role: string): void {
  localStorage.setItem(COOKIE_TOKEN, token);
  localStorage.setItem(COOKIE_ROLE, role);
  document.cookie = `${COOKIE_TOKEN}=${token}; path=/; SameSite=Lax`;
  document.cookie = `${COOKIE_ROLE}=${role}; path=/; SameSite=Lax`;
}

export function clearSession(): void {
  localStorage.removeItem(COOKIE_TOKEN);
  localStorage.removeItem(COOKIE_ROLE);
  document.cookie = `${COOKIE_TOKEN}=; path=/; max-age=0`;
  document.cookie = `${COOKIE_ROLE}=; path=/; max-age=0`;
}

export function getToken(): string | null {
  return localStorage.getItem(COOKIE_TOKEN);
}

export function getRole(): string | null {
  return localStorage.getItem(COOKIE_ROLE);
}
