import type { LoanResponse } from "@/lib/types";
import { C } from "@/lib/tokens";

export function StatusBadge({ status }: { status: LoanResponse["status"] }) {
  const map = {
    Pending:  { label: "Pendiente", bg: C.warningSubt, color: C.warning },
    Approved: { label: "Aprobado",  bg: C.successSubt, color: C.success },
    Rejected: { label: "Rechazado", bg: C.dangerSubt,  color: C.danger  },
  };
  const { label, bg, color } = map[status];
  return (
    <span style={{ display: "inline-block", fontSize: "12px", fontWeight: "600", padding: "2px 10px", borderRadius: "9999px", backgroundColor: bg, color }}>
      {label}
    </span>
  );
}
