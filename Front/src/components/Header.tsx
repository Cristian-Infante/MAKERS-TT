"use client";
import { useEffect, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { getToken } from "@/lib/storage";
import { decodeToken } from "@/lib/auth";
import { Button } from "@/components/Button";

export default function Header() {
  const { logout } = useAuth();
  const [displayName, setDisplayName] = useState("");

  useEffect(() => {
    const token = getToken();
    if (!token) return;
    const payload = decodeToken(token);
    if (!payload) return;
    const name =
      (payload["unique_name"] as string) ||
      (payload["name"] as string) ||
      (payload["email"] as string) ||
      "";
    setDisplayName(name);
  }, []);

  return (
    <header
      className="shadow-header sticky-hdr"
      style={{ backgroundColor: "#FEFCF9", borderBottom: "1px solid #DDD8D0" }}
    >
      <div
        className="d-flex items-c justify-b page-main"
        style={{ paddingTop: 0, paddingBottom: 0, height: "56px" }}
      >
        <span style={{ fontWeight: "600", fontSize: "15px", color: "#1C1917", letterSpacing: "-0.01em" }}>
          Makers Loans
        </span>

        <div className="d-flex items-c gap-3">
          {displayName && (
            <span className="hdr-name">Hola, {displayName}</span>
          )}
          <Button
            id="btn-logout"
            variant="ghost"
            onClick={logout}
            style={{ height: "32px", fontSize: "13px" }}
          >
            <span className="logout-text">Cerrar sesión</span>
            <span className="logout-icon" aria-label="Cerrar sesión">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                <polyline points="16 17 21 12 16 7" />
                <line x1="21" y1="12" x2="9" y2="12" />
              </svg>
            </span>
          </Button>
        </div>
      </div>
    </header>
  );
}
