import type { CSSProperties } from "react";

export const C = {
  bg:          "#F5F3EF",
  surface:     "#FEFCF9",
  border:      "#DDD8D0",
  textPrimary: "#1C1917",
  textMuted:   "#79736C",
  primary:     "#2E4057",
  danger:      "#8B1A2A",
  dangerSubt:  "#FDF2F3",
  success:     "#1E5631",
  successSubt: "#F0F7F2",
  warning:     "#92640A",
  warningSubt: "#FDF8EE",
} as const;

export const lbl: CSSProperties = {
  display: "block", fontSize: "11px", fontWeight: "600", color: "#79736C",
  marginBottom: "6px", textTransform: "uppercase", letterSpacing: "0.06em",
};
