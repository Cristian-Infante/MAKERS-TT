"use client";
import { useEffect, useRef, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useAdminLoans } from "@/hooks/useAdminLoans";
import { apiClient } from "@/lib/apiClient";
import type { LoanResponse } from "@/lib/types";
import { C, lbl } from "@/lib/tokens";
import { StatusBadge } from "@/components/StatusBadge";
import { Spinner } from "@/components/Spinner";
import { Button } from "@/components/Button";
import { AlertMessage } from "@/components/AlertMessage";
import { DataTable } from "@/components/DataTable";
import type { Column } from "@/components/DataTable";

export default function AdminPage() {
  const { checkSession } = useAuth();
  const { loans, isLoading, error, mutate } = useAdminLoans();
  const [rejectingLoan, setRejectingLoan] = useState<LoanResponse | null>(null);
  const [reason, setReason]               = useState("");
  const [processingId, setProcessingId]   = useState<string | null>(null);
  const [actionError, setActionError]     = useState("");
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => { checkSession(); }, []);
  useEffect(() => { if (rejectingLoan) textareaRef.current?.focus(); }, [rejectingLoan]);
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === "Escape") closeModal(); };
    if (rejectingLoan) window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [rejectingLoan]);

  function closeModal() { setRejectingLoan(null); setReason(""); setActionError(""); }

  async function approve(id: string) {
    setProcessingId(id); setActionError("");
    try { await apiClient.patch(`/api/v1/Loans/${id}`, { status: "Approved" }); await mutate(); }
    catch (err) { setActionError(err instanceof Error ? err.message : "Error al aprobar"); }
    finally { setProcessingId(null); }
  }

  async function submitReject(e: React.FormEvent) {
    e.preventDefault();
    if (!rejectingLoan) return;
    setProcessingId(rejectingLoan.id); setActionError("");
    try { await apiClient.patch(`/api/v1/Loans/${rejectingLoan.id}`, { status: "Rejected", reason }); await mutate(); closeModal(); }
    catch (err) { setActionError(err instanceof Error ? err.message : "Error al rechazar"); setProcessingId(null); }
  }

  return (
    <>
      <section className="section-card shadow-card animate-fade-in">
        <div className="section-hd">
          <h2 style={{ fontSize: "14px", fontWeight: "600", color: C.textPrimary }}>Todos los préstamos</h2>
          {!isLoading && loans.length > 0 && (
            <span style={{ fontSize: "12px", color: C.textMuted }}>{loans.length} {loans.length === 1 ? "registro" : "registros"}</span>
          )}
        </div>

        {actionError && <AlertMessage variant="danger" duration={4000} onDismiss={() => setActionError("")} className="mx-resp" style={{ marginTop: "1rem" }}>{actionError}</AlertMessage>}
        {isLoading && <Spinner />}
        {error && <AlertMessage variant="danger" className="mx-resp" style={{ margin: "1rem" }}>Error al cargar préstamos.</AlertMessage>}
        {!isLoading && !error && loans.length === 0 && (
          <p className="px-resp" style={{ paddingTop: "2.5rem", paddingBottom: "2.5rem", fontSize: "13px", color: C.textMuted }}>No hay préstamos registrados.</p>
        )}

        {loans.length > 0 && (
          <>
            <DataTable
              items={loans}
              keyFn={(l) => l.id}
              visibilityClass="only-desktop"
              minWidth="720px"
              columns={[
                { header: "Usuario",   tdStyle: { color: C.textPrimary, fontWeight: "500" },                                                                                      cell: (l) => l.userName },
                { header: "Monto",     tdStyle: { color: C.textPrimary },                                                                                                         cell: (l) => `$${l.amount.toLocaleString("es-CO")}` },
                { header: "Plazo",     tdStyle: { color: C.textMuted },                                                                                                           cell: (l) => `${l.termInMonths} meses` },
                { header: "Propósito", tdStyle: { color: C.textMuted, maxWidth: "160px", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" },                   cell: (l) => l.purpose },
                { header: "Fecha",     tdStyle: { color: C.textMuted, whiteSpace: "nowrap" },                                                                                     cell: (l) => new Date(l.createdAt).toLocaleDateString("es-CO") },
                { header: "Estado",    cell: (l) => <StatusBadge status={l.status} /> },
                { header: "Acciones",  cell: (l) => l.status === "Pending" ? (
                  <div className="d-flex gap-2">
                    <Button variant="success" id={`btn-approve-${l.id}`} onClick={() => approve(l.id)} disabled={processingId === l.id} style={{ height: "28px", fontSize: "12px", padding: "0 10px" }}>
                      {processingId === l.id ? "..." : "Aprobar"}
                    </Button>
                    <Button variant="danger" id={`btn-reject-${l.id}`} onClick={() => { setRejectingLoan(l); setActionError(""); }} disabled={processingId === l.id} style={{ height: "28px", fontSize: "12px", padding: "0 10px" }}>
                      Rechazar
                    </Button>
                  </div>
                ) : null },
              ]}
            />

            {/* Tarjetas <768px */}
            <div className="only-mobile card-list">
              {loans.map(loan => {
                const isPending = loan.status === "Pending";
                const isProc    = processingId === loan.id;
                return (
                  <div key={loan.id} style={{ padding: "14px 16px" }}>
                    <div className="d-flex items-s justify-b" style={{ marginBottom: "6px" }}>
                      <div>
                        <p style={{ fontSize: "14px", fontWeight: "600", color: C.textPrimary }}>{loan.userName}</p>
                        <p style={{ fontSize: "13px", color: C.textPrimary, marginTop: "1px" }}>${loan.amount.toLocaleString("es-CO")}</p>
                      </div>
                      <StatusBadge status={loan.status} />
                    </div>
                    <div className="d-flex-col gap-1" style={{ marginBottom: isPending ? "10px" : 0 }}>
                      <span style={{ fontSize: "12px", color: C.textMuted }}>Plazo: {loan.termInMonths} meses</span>
                      <span style={{ fontSize: "12px", color: C.textMuted }}>Propósito: {loan.purpose}</span>
                      <span style={{ fontSize: "12px", color: C.textMuted }}>Fecha: {new Date(loan.createdAt).toLocaleDateString("es-CO")}</span>
                    </div>
                    {isPending && (
                      <div className="d-flex gap-2">
                        <Button variant="success" onClick={() => approve(loan.id)} disabled={isProc} className="flex-1" style={{ height: "34px", fontSize: "13px" }}>
                          {isProc ? "..." : "Aprobar"}
                        </Button>
                        <Button variant="danger" onClick={() => { setRejectingLoan(loan); setActionError(""); }} disabled={isProc} className="flex-1" style={{ height: "34px", fontSize: "13px" }}>
                          Rechazar
                        </Button>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          </>
        )}
      </section>

      {/* ── Modal ── */}
      {rejectingLoan && (
        <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) closeModal(); }}>
          <div role="dialog" aria-modal="true" aria-labelledby="modal-title" className="modal-panel shadow-modal animate-scale-in">
            <div className="d-flex items-c justify-b" style={{ marginBottom: "1rem" }}>
              <div>
                <h3 id="modal-title" style={{ fontSize: "15px", fontWeight: "600", color: C.textPrimary }}>Rechazar préstamo</h3>
                <p style={{ fontSize: "13px", color: C.textMuted, marginTop: "2px" }}>{rejectingLoan.userName} · ${rejectingLoan.amount.toLocaleString("es-CO")}</p>
              </div>
              <Button variant="ghost" onClick={closeModal} aria-label="Cerrar" style={{ width: "32px", height: "32px", padding: 0, minWidth: 0 }}>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </Button>
            </div>
            {actionError && <AlertMessage variant="danger" duration={4000} onDismiss={() => setActionError("")} style={{ marginBottom: "1rem" }}>{actionError}</AlertMessage>}
            <form onSubmit={submitReject} style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
              <div>
                <label htmlFor="reason" style={lbl}>Motivo de rechazo</label>
                <textarea id="reason" ref={textareaRef} required rows={3} value={reason} onChange={e => setReason(e.target.value)} placeholder="Describe el motivo del rechazo..." />
              </div>
              <div className="modal-btns">
                <Button variant="ghost" type="button" onClick={closeModal} className="modal-btn" style={{ height: "38px" }}>Cancelar</Button>
                <Button variant="danger" type="submit" disabled={processingId === rejectingLoan.id} className="modal-btn" style={{ height: "38px" }}>
                  {processingId === rejectingLoan.id ? "Enviando..." : "Confirmar rechazo"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
