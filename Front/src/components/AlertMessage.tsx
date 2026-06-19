import { useEffect, useRef } from "react";
import type { CSSProperties, ReactNode } from "react";
import { C } from "@/lib/tokens";

interface AlertMessageProps {
  variant: "danger" | "success";
  children: ReactNode;
  className?: string;
  style?: CSSProperties;
  duration?: number;
  onDismiss?: () => void;
}

export function AlertMessage({ variant, children, className = "", style, duration, onDismiss }: AlertMessageProps) {
  const dismissRef = useRef(onDismiss);
  useEffect(() => { dismissRef.current = onDismiss; }, [onDismiss]);

  useEffect(() => {
    if (!duration) return;
    const id = setTimeout(() => dismissRef.current?.(), duration);
    return () => clearTimeout(id);
  }, [duration]);

  const isSuccess = variant === "success";
  const color     = isSuccess ? C.success : C.danger;
  const bg        = isSuccess ? C.successSubt : C.dangerSubt;
  return (
    <div
      role={isSuccess ? "status" : "alert"}
      className={`animate-fade-in${className ? ` ${className}` : ""}`}
      style={{ padding: "10px 14px", backgroundColor: bg, borderLeft: `4px solid ${color}`, color, fontSize: "13px", borderRadius: "0 4px 4px 0", ...style }}
    >
      {children}
    </div>
  );
}
