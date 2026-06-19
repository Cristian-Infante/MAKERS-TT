import type { ButtonHTMLAttributes } from "react";

type ButtonVariant = "primary" | "danger" | "success" | "ghost";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
}

export function Button({ variant = "primary", className = "", children, ...props }: ButtonProps) {
  const cls = `btn btn-${variant}${className ? ` ${className}` : ""}`;
  return <button className={cls} {...props}>{children}</button>;
}
