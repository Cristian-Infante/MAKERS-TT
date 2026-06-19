import type { CSSProperties, ReactNode } from "react";
import { C } from "@/lib/tokens";

export interface Column<T> {
  header: string;
  cell: (item: T) => ReactNode;
  tdStyle?: CSSProperties;
}

interface DataTableProps<T> {
  items: T[];
  keyFn: (item: T) => string;
  columns: Column<T>[];
  minWidth?: string;
  visibilityClass?: string;
  hPadding?: string;
}

export function DataTable<T>({
  items,
  keyFn,
  columns,
  minWidth = "600px",
  visibilityClass = "only-tablet-up",
  hPadding = "16px",
}: DataTableProps<T>) {
  const thStyle: CSSProperties = {
    textAlign: "left", padding: `10px ${hPadding}`, fontSize: "11px",
    fontWeight: "600", textTransform: "uppercase", letterSpacing: "0.06em",
    color: C.textMuted, backgroundColor: C.bg, borderBottom: `1px solid ${C.border}`,
  };

  return (
    <div className={`${visibilityClass} ovf-xa`}>
      <table style={{ width: "100%", minWidth, borderCollapse: "collapse", fontSize: "13px" }}>
        <thead>
          <tr>{columns.map((col) => <th key={col.header} style={thStyle}>{col.header}</th>)}</tr>
        </thead>
        <tbody>
          {items.map((item, i) => (
            <tr
              key={keyFn(item)}
              style={{ borderBottom: i < items.length - 1 ? `1px solid ${C.border}` : "none" }}
              onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = C.bg)}
              onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = "transparent")}
            >
              {columns.map((col) => (
                <td key={col.header} style={{ padding: `12px ${hPadding}`, ...col.tdStyle }}>
                  {col.cell(item)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
