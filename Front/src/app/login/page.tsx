"use client";
import { useState } from "react";
import Link from "next/link";
import { useAuth } from "@/hooks/useAuth";
import { APP_ROUTES } from "@/constants/routes";

function EyeIcon({ open }: { open: boolean }) {
  return open ? (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
      <circle cx="12" cy="12" r="3"/>
    </svg>
  ) : (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
      <line x1="1" y1="1" x2="23" y2="23"/>
    </svg>
  );
}

export default function LoginPage() {
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await login(email, password);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al iniciar sesión");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main
      style={{
        minHeight: "100vh",
        backgroundColor: "#F5F3EF",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: "1rem",
      }}
    >
      <div
        className="shadow-auth animate-scale-in"
        style={{
          backgroundColor: "#FEFCF9",
          borderRadius: "12px",
          border: "1px solid #DDD8D0",
          padding: "2rem",
          width: "100%",
          maxWidth: "384px",
        }}
      >
        {/* Brand */}
        <div style={{ marginBottom: "1.75rem" }}>
          <h1
            style={{
              fontSize: "20px",
              fontWeight: "600",
              color: "#1C1917",
              marginBottom: "2px",
            }}
          >
            Makers Loans
          </h1>
          <p style={{ fontSize: "13px", color: "#79736C" }}>
            Inicia sesión en tu cuenta
          </p>
        </div>

        {error && (
          <div
            className="animate-fade-in"
            role="alert"
            style={{
              marginBottom: "1rem",
              backgroundColor: "#FDF2F3",
              borderLeft: "4px solid #8B1A2A",
              color: "#8B1A2A",
              fontSize: "13px",
              padding: "0.75rem 1rem",
              borderRadius: "0 4px 4px 0",
            }}
          >
            {error}
          </div>
        )}

        <form onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
          <div>
            <label
              htmlFor="email"
              style={{
                display: "block",
                fontSize: "12px",
                fontWeight: "600",
                color: "#79736C",
                marginBottom: "6px",
                textTransform: "uppercase",
                letterSpacing: "0.05em",
              }}
            >
              Correo electrónico
            </label>
            <input
              id="email"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="correo@ejemplo.com"
            />
          </div>

          <div>
            <label
              htmlFor="password"
              style={{
                display: "block",
                fontSize: "12px",
                fontWeight: "600",
                color: "#79736C",
                marginBottom: "6px",
                textTransform: "uppercase",
                letterSpacing: "0.05em",
              }}
            >
              Contraseña
            </label>
            <div style={{ position: "relative" }}>
              <input
                id="password"
                type={showPassword ? "text" : "password"}
                required
                minLength={3}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                style={{ paddingRight: "2.75rem" }}
              />
              <button
                type="button"
                onClick={() => setShowPassword((v) => !v)}
                aria-label={showPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
                style={{
                  position: "absolute",
                  right: "0.75rem",
                  top: "50%",
                  transform: "translateY(-50%)",
                  background: "none",
                  border: "none",
                  cursor: "pointer",
                  color: "#79736C",
                  display: "flex",
                  alignItems: "center",
                  padding: 0,
                }}
              >
                <EyeIcon open={showPassword} />
              </button>
            </div>
          </div>

          <button
            id="btn-login"
            type="submit"
            disabled={loading}
            className="btn btn-primary"
            style={{ width: "100%", height: "40px", fontSize: "14px", marginTop: "4px" }}
          >
            {loading ? (
              <>
                <div
                  className="animate-spin"
                  style={{
                    width: "14px",
                    height: "14px",
                    borderRadius: "50%",
                    border: "2px solid rgba(245,243,239,0.3)",
                    borderTopColor: "#F5F3EF",
                  }}
                />
                Ingresando...
              </>
            ) : (
              "Entrar"
            )}
          </button>
        </form>

        <p
          style={{
            fontSize: "13px",
            color: "#79736C",
            textAlign: "center",
            marginTop: "1.25rem",
          }}
        >
          ¿No tienes cuenta?{" "}
          <Link
            href={APP_ROUTES.REGISTER}
            style={{ color: "#2E4057", fontWeight: "600", textDecoration: "none" }}
          >
            Regístrate
          </Link>
        </p>
      </div>
    </main>
  );
}
