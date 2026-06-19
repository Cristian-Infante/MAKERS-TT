import { C } from "@/lib/tokens";

export function Spinner() {
  return (
    <div className="d-flex justify-c" style={{ padding: "3rem 0" }}>
      <div className="animate-spin" style={{ width: "24px", height: "24px", borderRadius: "50%", border: `2px solid ${C.border}`, borderTopColor: C.primary }} />
    </div>
  );
}
