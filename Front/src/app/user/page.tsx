"use client";
import { useEffect, useState } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useMyLoans } from "@/hooks/useMyLoans";
import { apiClient } from "@/lib/apiClient";
import type { LoanResponse } from "@/lib/types";
import { C, lbl } from "@/lib/tokens";
import { StatusBadge } from "@/components/StatusBadge";
import { Spinner } from "@/components/Spinner";
import { Button } from "@/components/Button";
import { AlertMessage } from "@/components/AlertMessage";
import { DataTable } from "@/components/DataTable";
import type { Column } from "@/components/DataTable";

const tableColumns: Column<LoanResponse>[] = [
  { header: "Monto",     tdStyle: { fontWeight: "500", color: C.textPrimary, whiteSpace: "nowrap" },                                                           cell: (l) => `$${l.amount.toLocaleString("es-CO")}` },
  { header: "Plazo",     tdStyle: { color: C.textMuted, whiteSpace: "nowrap" },                                                                                cell: (l) => `${l.termInMonths} meses` },
  { header: "Propósito", tdStyle: { color: C.textMuted, maxWidth: "200px", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" },               cell: (l) => l.purpose },
  { header: "Fecha",     tdStyle: { color: C.textMuted, whiteSpace: "nowrap" },                                                                                cell: (l) => new Date(l.createdAt).toLocaleDateString("es-CO") },
  { header: "Estado",    cell: (l) => <StatusBadge status={l.status} /> },
];

function EmptyLoans() {
  return (
    <div className="d-flex-col items-c justify-c text-c" style={{ padding: "4rem 1.5rem" }}>
      <svg className="animate-fade-in" style={{ width: "36px", height: "36px", color: C.border, marginBottom: "12px" }} fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
      <p style={{ fontSize: "13px", fontWeight: "500", color: C.textPrimary }}>Sin préstamos aún</p>
      <p style={{ fontSize: "12px", color: C.textMuted, marginTop: "4px" }}>Usa el formulario de arriba para solicitar uno.</p>
    </div>
  );
}


export default function UserPage() {
  const { checkSession } = useAuth();
  const { loans, isLoading, error, mutate } = useMyLoans();
  const [amount, setAmount]       = useState("");
  const [term, setTerm]           = useState("12");
  const [purpose, setPurpose]     = useState("");
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess]     = useState(false);

  useEffect(() => { checkSession(); }, []);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setFormError(""); setSuccess(false); setSubmitting(true);
    try {
      await apiClient.post("/api/v1/Loans", { amount: parseFloat(amount), termInMonths: parseInt(term), purpose });
      await mutate();
      setAmount(""); setTerm("12"); setPurpose("");
      setSuccess(true);
    } catch (err) {
      setFormError(err instanceof Error ? err.message : "Error al solicitar préstamo");
    } finally { setSubmitting(false); }
  }

  return (
    <div className="d-flex-col gap-6">

      {/* ── Solicitar préstamo ── */}
      <section className="section-card shadow-card animate-fade-in">
        <div className="section-hd">
          <h2 style={{ fontSize: "14px", fontWeight: "600", color: C.textPrimary }}>Solicitar préstamo</h2>
        </div>
        <div className="section-bd">
          {formError && <AlertMessage variant="danger" duration={4000} onDismiss={() => setFormError("")} style={{ marginBottom: "1.25rem" }}>{formError}</AlertMessage>}
          {success  && <AlertMessage variant="success" duration={4000} onDismiss={() => setSuccess(false)} style={{ marginBottom: "1.25rem" }}>Préstamo solicitado con éxito.</AlertMessage>}
          <form onSubmit={onSubmit}>
            <div className="form-grid">
              <div>
                <label htmlFor="amount" style={lbl}>Monto</label>
                <input id="amount" type="number" required min="0.01" step="0.01" value={amount} onChange={(e) => setAmount(e.target.value)} placeholder="0.00" />
              </div>
              <div>
                <label htmlFor="term" style={lbl}>Plazo (meses)</label>
                <input id="term" type="number" required min="1" max="120" value={term} onChange={(e) => setTerm(e.target.value)} />
              </div>
              <div>
                <label htmlFor="purpose" style={lbl}>Propósito</label>
                <input id="purpose" type="text" required maxLength={500} value={purpose} onChange={(e) => setPurpose(e.target.value)} placeholder="Ej. compra de equipo" />
              </div>
            </div>
            <div className="d-flex justify-e">
              <Button id="btn-submit-loan" type="submit" disabled={submitting} style={{ height: "38px", paddingLeft: "1.5rem", paddingRight: "1.5rem" }}>
                {submitting
                  ? <><div className="animate-spin" style={{ width: "13px", height: "13px", borderRadius: "50%", border: "2px solid rgba(245,243,239,0.3)", borderTopColor: "#F5F3EF" }} />Enviando...</>
                  : "Solicitar"}
              </Button>
            </div>
          </form>
        </div>
      </section>

      {/* ── Mis préstamos ── */}
      <section className="section-card shadow-card animate-fade-in">
        <div className="section-hd">
          <h2 style={{ fontSize: "14px", fontWeight: "600", color: C.textPrimary }}>Mis préstamos</h2>
          {!isLoading && loans.length > 0 && (
            <span style={{ fontSize: "12px", color: C.textMuted }}>{loans.length} {loans.length === 1 ? "registro" : "registros"}</span>
          )}
        </div>

        {isLoading && <Spinner />}
        {!isLoading && error && (
          <AlertMessage variant="danger" style={{ margin: "16px" }}>Error al cargar los préstamos.</AlertMessage>
        )}
        {!isLoading && !error && loans.length === 0 && <EmptyLoans />}
        {!isLoading && !error && loans.length > 0 && (
          <>
            <DataTable items={loans} keyFn={(l) => l.id} columns={tableColumns} minWidth="520px" visibilityClass="only-tablet-up" hPadding="24px" />
            {/* Tarjetas <640px */}
            <div className="only-mobile-sm card-list">
              {loans.map(loan => (
                <div key={loan.id} style={{ padding: "14px 16px" }}>
                  <div className="d-flex items-s justify-b" style={{ marginBottom: "8px" }}>
                    <span style={{ fontSize: "15px", fontWeight: "600", color: C.textPrimary }}>${loan.amount.toLocaleString("es-CO")}</span>
                    <StatusBadge status={loan.status} />
                  </div>
                  <div className="d-flex-col gap-1">
                    <span style={{ fontSize: "12px", color: C.textMuted }}>Plazo: {loan.termInMonths} meses</span>
                    <span style={{ fontSize: "12px", color: C.textMuted }}>Propósito: {loan.purpose}</span>
                    <span style={{ fontSize: "12px", color: C.textMuted }}>Fecha: {new Date(loan.createdAt).toLocaleDateString("es-CO")}</span>
                  </div>
                </div>
              ))}
            </div>
          </>
        )}
      </section>
    </div>
  );
}
